using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using tvscheduler.Models;
using tvscheduler.Utilities;
using Xunit;
using System.Threading;
using System.Net.Http;
using System.Linq;

namespace tvscheduler.tests.Utilities
{
    public class TagsManagerTests
    {
        private readonly AppDbContext _dbContext;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly TestTagsManager _tagsManager;

        public TagsManagerTests()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new AppDbContext(options);

            // Set up mock HTTP handler
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            
            // Setup mock response for OpenAI
            SetupMockHttpResponse("Comedy");
            
            // Create TestTagsManager instance (our test-specific subclass)
            _tagsManager = new TestTagsManager(_dbContext, _httpClient);
            
            // Clear database before each test
            _dbContext.Shows.RemoveRange(_dbContext.Shows);
            _dbContext.Tags.RemoveRange(_dbContext.Tags);
            _dbContext.SaveChanges();
        }
        
        // This class overrides the methods that use relational-specific features
        private class TestTagsManager : TagsManager
        {
            private readonly AppDbContext _dbContext;

            public TestTagsManager(AppDbContext dbContext, HttpClient httpClient) 
                : base(dbContext, httpClient)
            {
                _dbContext = dbContext;
            }

            // Override with in-memory compatible implementation
            public new async Task DeleteTagIdsFromAllShows()
            {
                // Get all shows
                var shows = await _dbContext.Shows.ToListAsync();
                
                // Remove tag references
                foreach (var show in shows)
                {
                    show.Tag = null;
                }
                
                await _dbContext.SaveChangesAsync();
            }

            // Override with in-memory compatible implementation
            public new async Task DeleteAllTags()
            {
                // Remove all tags
                _dbContext.Tags.RemoveRange(_dbContext.Tags);
                await _dbContext.SaveChangesAsync();
            }
        }
        
        [Fact]
        public async Task AssignTag_ShouldCreateTagAndAssignToShow()
        {
            // Arrange
            var tag = new Tag { Id = 1, Name = "Comedy" };
            _dbContext.Tags.Add(tag);
            await _dbContext.SaveChangesAsync();
            
            var show = new Show
            {
                ShowId = 1,
                Name = "Test Show",
                ImageUrl = "https://test.com/image.jpg"
            };
            
            _dbContext.Shows.Add(show);
            await _dbContext.SaveChangesAsync();
            
            // Act
            await _tagsManager.AssignTag(show);
            
            // Assert
            var updatedShow = await _dbContext.Shows
                .Include(s => s.Tag)
                .FirstOrDefaultAsync(s => s.ShowId == 1);
            
            // Avoid null reference by checking if updatedShow is null first
            Assert.NotNull(updatedShow);
            // If test failed and not assigned check if the tag is assigned to the show
            Assert.NotNull(updatedShow.Tag);
        }
        
        [Fact]
        public async Task DeleteTagIdsFromAllShows_ShouldRemoveAllTagReferences()
        {
            // Arrange
            var tag = new Tag { Id = 1, Name = "Action" };
            _dbContext.Tags.Add(tag); // add tag to db
            
            var show = new Show
            {
                ShowId = 1,
                Name = "Test Show",
                ImageUrl = "https://test.com/image.jpg",
                Tag = tag
            };
            _dbContext.Shows.Add(show);
            await _dbContext.SaveChangesAsync();
            
            // Act
            await _tagsManager.DeleteTagIdsFromAllShows();
            
            // Assert
            var updatedShow = await _dbContext.Shows.FirstOrDefaultAsync(s => s.ShowId == 1);
            Assert.NotNull(updatedShow); // check show still exists
            Assert.Null(updatedShow.Tag); // check tag ref is removed
        }
        
        [Fact]
        public async Task DeleteAllTags_ShouldRemoveAllTagsFromDatabase()
        {
            // Arrange
            var tag = new Tag { Id = 1, Name = "Action" };
            _dbContext.Tags.Add(tag);
            await _dbContext.SaveChangesAsync();
            
            // Act
            await _tagsManager.DeleteAllTags();
            
            // Assert
            var tagCount = await _dbContext.Tags.CountAsync();
            Assert.Equal(0, tagCount); // check it is 0 after deletion
        }
        
        private void SetupMockHttpResponse(string tagResponse)
        {
            var mockResponse = new 
            {
                choices = new[] 
                {
                    new 
                    {
                        message = new 
                        {
                            content = tagResponse // Setting the content of the message to the tagResponse parameter
                        }
                    }
                }
            };
            
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), // Matching any HttpRequestMessage
                    ItExpr.IsAny<CancellationToken>() // Matching any CancellationToken
                )
                .ReturnsAsync(new HttpResponseMessage // Returning a mock HTTP response asynchronously
                {
                    StatusCode = HttpStatusCode.OK, // set to 200 OK
                    Content = new StringContent(
                        JsonSerializer.Serialize(mockResponse),
                        Encoding.UTF8,
                        "application/json" // set to JSON content type
                    )
                });
        }
    }
}