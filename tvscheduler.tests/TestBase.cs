using Microsoft.AspNetCore.Identity; // authentication and authorization
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected; // Mocking protected methods, http handlers
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using tvscheduler.Models;

namespace tvscheduler.tests;

/// Base class that provides common test functionality
public abstract class TestBase // other tests to inherit
{
    protected AppDbContext CreateInMemoryDbContext() // !need real database so test  dont interfere with each other
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // each test gets unique db
            .Options;
            
        var dbContext = new AppDbContext(options);
        
        // Clear any existing data
        dbContext.Database.EnsureDeleted(); // if it exists delete it
        dbContext.Database.EnsureCreated(); // create new db for testing
        
        return dbContext;
    }
    
    protected Mock<UserManager<User>> CreateMockUserManager() // mock user manager for testing authen/authorize
    {
        var store = new Mock<IUserStore<User>>();
        var mgr = new Mock<UserManager<User>>(
            store.Object, // the mock user store instance
            // accpet all the other parameters
            It.IsAny<IOptions<IdentityOptions>>(),
            It.IsAny<IPasswordHasher<User>>(),
            It.IsAny<IEnumerable<IUserValidator<User>>>(),
            It.IsAny<IEnumerable<IPasswordValidator<User>>>(),
            It.IsAny<ILookupNormalizer>(),
            It.IsAny<IdentityErrorDescriber>(),
            It.IsAny<IServiceProvider>(),
            It.IsAny<ILogger<UserManager<User>>>());
            
        // Setup common methods
        // configure the mock to return a successful result for all methods
        mgr.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success); 
            
        mgr.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success); 
            
        mgr.Setup(x => x.DeleteAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);
            
        return mgr;
    }
    
    protected Mock<IConfiguration> CreateMockConfiguration() // with pre-set values for testing
    {
        // Setup configuration interface and JWT settings
        var config = new Mock<IConfiguration>();
        config.Setup(x => x["Jwt:Key"]).Returns("test-key-with-32-chars-long-for-testing");
        config.Setup(x => x["Jwt:Issuer"]).Returns("test-issuer");
        config.Setup(x => x["Jwt:Audience"]).Returns("test-audience");
        
        return config;
    }
    
    protected HttpClient CreateMockHttpClient(HttpResponseMessage responseMessage) // that returns predefined responses
    {
        var mockHandler = new Mock<HttpMessageHandler>();

        // Setup the SendAsync method to return the response message and accept any request message
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);
            
        return new HttpClient(mockHandler.Object); // Creates and returns an HTTP client with the mocked handler
    }
} 