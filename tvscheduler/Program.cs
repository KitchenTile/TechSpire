using System.Security.Claims;
using System.Text;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using tvscheduler;
using tvscheduler.Models;
using tvscheduler.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc( "v1", new OpenApiInfo
    {
        Title = "DM Web API's",
        Version = "v1"
    });

    var security = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    };

    x.AddSecurityDefinition("Bearer", security);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };
    x.AddSecurityRequirement(securityRequirement);
});


// protectagaint circular dependencies when rendering json
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.MaxDepth = 64; // Increase depth limit if needed
});


// ADJUST USER CREATION REGEX
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 0;
});


builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)),
        NameClaimType = ClaimTypes.NameIdentifier

    };
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});


// database <-> ORM (Entity Framework) DI
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);



builder.Services.AddHangfire(opts =>
{
    opts.UseStorage(new MySqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")+"Allow User Variables=true;",
        new MySqlStorageOptions() {TablesPrefix = "hangfire_"}));
    opts.UseColouredConsoleLogProvider();
});
builder.Services.AddHangfireServer();


// custom services added to DI pipeline
builder.Services.AddScoped<UpdateChannelSchedule>();
builder.Services.AddScoped<TagsManager>();
builder.Services.AddSingleton<HangfireJobs>();
builder.Services.AddSingleton<TodaysShowsCache>();
builder.Services.AddScoped<RecommendationGeneratorGlobal>();
builder.Services.AddScoped<RecommendationGeneratorIndividual>();
builder.Services.AddScoped<TodaysShowsCacheUpdate>();

var app = builder.Build();


// run on program startup
using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    var showsCache = serviceScope.ServiceProvider.GetRequiredService<TodaysShowsCache>();
    var showsCacheUpdate = serviceScope.ServiceProvider.GetRequiredService<TodaysShowsCacheUpdate>();
    if (dbContext.Channels.IsNullOrEmpty())
    {
        var databaseInit = new DatabaseChannelsInit(dbContext);
        databaseInit.SeedDatabase(); // adding hard coded channels to the database if no channels exist
    }

    if (!dbContext.ShowEvents.IsNullOrEmpty() && showsCache.GetCachedShows().IsNullOrEmpty())
    {
        await showsCacheUpdate.UpdateCachedShows();
    }
}


//HANGFIRE jobs
using (var scope = app.Services.CreateScope())
{
    var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>(); // hangfire service to handle jobs
    var hangfireJobs = scope.ServiceProvider.GetRequiredService<HangfireJobs>(); // class defining the jobs
    

    recurringJobs.AddOrUpdate("Update the channel schedule for two days",
        ()=> hangfireJobs.UpdateChannelScheduleJob(),
        Cron.Never());
    
    recurringJobs.AddOrUpdate("Find Tags For Untagged Shows",
        () => hangfireJobs.CheckDatabaseForUntagged(),
        Cron.Never());

    recurringJobs.AddOrUpdate("Reassign tags for all shows",
        () => hangfireJobs.ReassignAllTags(),
        Cron.Never());

    recurringJobs.AddOrUpdate("Set tag field in every show to NULL",
        () => hangfireJobs.DeleteTagIdsFromAllShows(),
        Cron.Never());

    recurringJobs.AddOrUpdate("Delete Tags From the Database",
        () => hangfireJobs.DeleteAllTags(),
        Cron.Never());
    
    recurringJobs.AddOrUpdate("Update Todays Shows Cache",
        () => hangfireJobs.UpdateTodaysShowsCache(),
        Cron.Daily());
    
    recurringJobs.AddOrUpdate("Update Global Recommendation",
        () => hangfireJobs.UpdateGlobalRecommendation(),
        Cron.Daily());
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();