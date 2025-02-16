using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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


// protectagaint circular dependencies when rendering json
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
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



builder.Services.AddScoped<UpdateChannelSchedule>();
builder.Services.AddScoped<TagsManager>();


var app = builder.Build();


// run on program startup
using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (dbContext.Channels.IsNullOrEmpty())
    {
        var databaseInit = new DatabaseChannelsInit(dbContext);
        databaseInit.SeedDatabase();
    }

    var channelUpdater = serviceScope.ServiceProvider.GetRequiredService<UpdateChannelSchedule>();
    await channelUpdater.UpdateDailySchedule();
    
    //var tagManager = serviceScope.ServiceProvider.GetRequiredService<TagsManager>();
    //await tagManager.CheckDatabaseForUntagged();
}

//HANGFIRE
//add the job to hangfire
using (var scope = app.Services.CreateScope())
{
    var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobs.AddOrUpdate("Fing Tags For Untagged Shows",
        ()=> scope.ServiceProvider.GetRequiredService<TagsManager>().CheckDatabaseForUntagged(),
        Cron.Never());
    
    recurringJobs.AddOrUpdate("Reassign tags for all shows",
        ()=> scope.ServiceProvider.GetRequiredService<TagsManager>().ReassignAllTags(),
        Cron.Never());
    
    recurringJobs.AddOrUpdate("Set tag field in every show to NULL",
        ()=> scope.ServiceProvider.GetRequiredService<TagsManager>().DeleteTagIdsFromAllShows(),
        Cron.Never());
    
    recurringJobs.AddOrUpdate("Delete Tags From the Databse",
        ()=> scope.ServiceProvider.GetRequiredService<TagsManager>().DeleteAllTags(),
        Cron.Never());
    
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
app.UseAuthorization();

app.MapControllers();

app.Run();