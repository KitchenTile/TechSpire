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

// Core service registration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Swagger configuration with JWT authentication
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DM Web API's",
        Version = "v1"
    });

    // JWT security definition
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

// Prevent circular reference issues in JSON serialization
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.MaxDepth = 64;
});

// Simplified password requirements for development
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 0;
});

// Identity and JWT setup
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)),
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

// CORS policy for frontend
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

// Database context setup with MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// Hangfire configuration for background jobs
builder.Services.AddHangfire(opts =>
{
    opts.UseStorage(new MySqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")+"Allow User Variables=true;",
        new MySqlStorageOptions() {TablesPrefix = "hangfire_"}));
    opts.UseColouredConsoleLogProvider();
});
builder.Services.AddHangfireServer();

// Application services registration
builder.Services.AddScoped<UpdateChannelSchedule>();
builder.Services.AddScoped<TagsManager>();
builder.Services.AddSingleton<HangfireJobs>();
builder.Services.AddSingleton<TodaysShowsCache>();
builder.Services.AddScoped<RecommendationGeneratorGlobal>();
builder.Services.AddScoped<RecommendationGeneratorIndividual>();
builder.Services.AddScoped<TodaysShowsCacheUpdate>();
builder.Services.AddScoped<ImageManager>();

var app = builder.Build();

// Initial database setup and cache population
using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    var showsCache = serviceScope.ServiceProvider.GetRequiredService<TodaysShowsCache>();
    var showsCacheUpdate = serviceScope.ServiceProvider.GetRequiredService<TodaysShowsCacheUpdate>();
    
    // Initialize channels if database is empty
    if (dbContext.Channels.IsNullOrEmpty())
    {
        var databaseInit = new DatabaseChannelsInit(dbContext);
        databaseInit.SeedDatabase();
    }

    // Populate cache if shows exist but cache is empty
    if (!dbContext.ShowEvents.IsNullOrEmpty() && showsCache.GetCachedShows().IsNullOrEmpty())
    {
        await showsCacheUpdate.UpdateCachedShows();
    }
}

// Hangfire job scheduling
using (var scope = app.Services.CreateScope())
{
    var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    var hangfireJobs = scope.ServiceProvider.GetRequiredService<HangfireJobs>();
    
    // Schedule background jobs
    recurringJobs.AddOrUpdate("Update the channel schedule for two days",
        () => hangfireJobs.UpdateChannelScheduleJob(),
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

    recurringJobs.AddOrUpdate("Clear Global Recommendations History",
        () => hangfireJobs.ClearGlobalRecommendationsHistory(),
        Cron.Never());
    
    recurringJobs.AddOrUpdate("Update Global Recommendation",
        () => hangfireJobs.UpdateGlobalRecommendation(),
        Cron.Daily());

    recurringJobs.AddOrUpdate("Clear Individual Recommendation History for all users",
        () => hangfireJobs.ClearIndividualRecommendationsHistory(),
        Cron.Never());

    recurringJobs.AddOrUpdate("Process Show Images",
        () => hangfireJobs.ProcessShowImages(),
        Cron.Daily());

    recurringJobs.AddOrUpdate("Delete All Resized Images",
        () => hangfireJobs.DeleteAllResizedImages(),
        Cron.Never());
}

// Development environment setup
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}

// Middleware pipeline configuration
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowFrontend");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();