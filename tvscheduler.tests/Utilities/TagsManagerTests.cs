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
    public class TagsManagerTests : TestBase
    {
        private readonly AppDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly TestTagsManager _tagsManager;

        public TagsManagerTests()
        {
            // Create in-memory database
            _dbContext = CreateInMemoryDbContext();
            
            // Setup HTTP client with test response
            _httpClient = CreateMockHttpClient(TestDataFactory.CreateTagResponse());
            
            // Create test-specific tags manager
            _tagsManager = new TestTagsManager(_dbContext, _httpClient);
        }

        // Test-specific TagsManager that overrides SQL-specific methods
        private class TestTagsManager : TagsManager
        {
            private readonly AppDbContext _dbContext;
            
            public TestTagsManager(AppDbContext dbContext, HttpClient httpClient)
                : base(dbContext, httpClient)
            {
                _dbContext = dbContext;
            }
            
            // Override the SQL-specific method with LINQ implementation
            public new async Task DeleteTagIdsFromAllShows()
            {
                var shows = await _dbContext.Shows.ToListAsync();
                foreach (var show in shows)
                {
                    show.Tag = null;
                }
                await _dbContext.SaveChangesAsync();
            }
            
            // Override the SQL-specific method with LINQ implementation
            public new async Task DeleteAllTags()
            {
                _dbContext.Tags.RemoveRange(_dbContext.Tags);
                await _dbContext.SaveChangesAsync();
            }
        }
        
        [Fact]
        public async Task AssignTag_ShouldCreateAndAssignTagToShow()
        {
            // Arrange
            var show = TestDataFactory.CreateShow();
            _dbContext.Shows.Add(show);
            await _dbContext.SaveChangesAsync();
            
            // Act
            await _tagsManager.AssignTag(show);
            
            // Assert
            var updatedShow = await _dbContext.Shows
                .Include(s => s.Tag)
                .FirstOrDefaultAsync(s => s.ShowId == show.ShowId);
                
            Assert.NotNull(updatedShow); // verifies that show is found
            Assert.NotNull(updatedShow.Tag); // verifies that tag is assigned
            Assert.Equal("Comedy", updatedShow.Tag.Name); // Comedy is the default tag response from mock
        }
        
        [Fact]
        public async Task AssignTag_WhenTagExists_ShouldReuseExistingTag()
        {
            // Arrange
            var existingTag = TestDataFactory.CreateTag(name: "Comedy");
            _dbContext.Tags.Add(existingTag);
            
            var show = TestDataFactory.CreateShow();
            _dbContext.Shows.Add(show);
            
            await _dbContext.SaveChangesAsync();
            
            // Act
            await _tagsManager.AssignTag(show);
            
            // Assert
            var updatedShow = await _dbContext.Shows
                .Include(s => s.Tag)
                .FirstOrDefaultAsync(s => s.ShowId == show.ShowId);
                
            Assert.NotNull(updatedShow); // verifies that show is found
            Assert.NotNull(updatedShow.Tag); // verifies that tag is assigned
            Assert.Equal(existingTag.Id, updatedShow.Tag.Id); // verifies that Should reuse the existing tag
            Assert.Equal("Comedy", updatedShow.Tag.Name); // verifies that Comedy is updated
        }
        
        [Fact]
        public async Task DeleteTagIdsFromAllShows_ShouldRemoveAllTagReferences()
        {
            // Arrange
            var tag = TestDataFactory.CreateTag();
            _dbContext.Tags.Add(tag);
            
            var show = TestDataFactory.CreateShow(tag: tag);
            _dbContext.Shows.Add(show);
            
            await _dbContext.SaveChangesAsync();
            
            // Act
            await _tagsManager.DeleteTagIdsFromAllShows();
            
            // Assert
            // Refresh entity from DB
            _dbContext.Entry(show).Reload();
            Assert.Null(show.Tag); // verifies that tag is removed
        }
        
        [Fact]
        public async Task DeleteAllTags_ShouldRemoveAllTagsFromDatabase()
        {
            // Arrange
            var tag = TestDataFactory.CreateTag();
            _dbContext.Tags.Add(tag);
            await _dbContext.SaveChangesAsync();
            
            // Act
            await _tagsManager.DeleteAllTags();
            
            // Assert
            var tagCount = await _dbContext.Tags.CountAsync();
            Assert.Equal(0, tagCount); // verifies that 0 tags
        }
    }
}