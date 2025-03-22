using Microsoft.EntityFrameworkCore;
using tvscheduler;
using tvscheduler.Models;

namespace tvscheduler.tests.Helpers;

public static class TestDbContextFactory
{
    public static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        var dbContext = new AppDbContext(options);
        
        // Seed with test data
        SeedTestData(dbContext);
        
        return dbContext;
    }
    
    private static void SeedTestData(AppDbContext dbContext)
    {
        // Add a test tag
        var tag = new Tag
        {
            Id = 1,
            Name = "Action"
        };
        dbContext.Tags.Add(tag);
        
        // Add a test channel
        var channel = new Channel
        {
            ChannelId = 1,
            Name = "Test Channel",
            Description = "Test Channel Description",
            Lcn = 1,
            LogoUrl = "https://test.com/logo.png",
            Tstv = true
        };
        dbContext.Channels.Add(channel);
        
        // Add a test show
        var show = new Show
        {
            ShowId = 1,
            Name = "Test Show",
            ImageUrl = "https://test.com/image.jpg", // Required property
            Tag = tag
        };
        dbContext.Shows.Add(show);
        
        // Add a test show event
        var showEvent = new ShowEvent
        {
            Id = 1,
            ChannelId = 1,
            Channel = channel,
            ShowId = 1,
            Show = show,
            TimeStart = 1710345600, // Unix timestamp as int
            Duration = 60,
            Description = "Test event description" // Required property
        };
        dbContext.ShowEvents.Add(showEvent);
        
        // Add a test user
        var user = new User
        {
            Id = "testUser123",
            UserName = "testuser"
        };
        dbContext.Users.Add(user);
        
        // Save changes before creating related entities
        dbContext.SaveChanges();
    }
}