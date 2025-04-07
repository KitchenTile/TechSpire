using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;
using tvscheduler.Utilities;
using Xunit;

namespace tvscheduler.tests.Utilities;

public class RecommendationGeneratorTests : TestBase
{
    private readonly AppDbContext _dbContext;
    private readonly TodaysShowsCache _todaysShowsCache;
    private readonly RecommendationGeneratorGlobal _globalGenerator;
    private readonly RecommendationGeneratorIndividual _individualGenerator;
    private readonly string _testUserId = "test-user-123";

    public RecommendationGeneratorTests()
    {
        _dbContext = CreateInMemoryDbContext();
        _todaysShowsCache = new TodaysShowsCache();
        _globalGenerator = new RecommendationGeneratorGlobal(_dbContext, _todaysShowsCache);
        _individualGenerator = new RecommendationGeneratorIndividual(_dbContext, _todaysShowsCache);
    }

    // Helper method to create test data - creates a tag and show, adds them to DB and updates cache
    private async Task<(Tag tag, Show show)> CreateTagAndShow(int showId = 1)
    {
        var tag = TestDataFactory.CreateTag();
        _dbContext.Tags.Add(tag);
        await _dbContext.SaveChangesAsync();

        var show = TestDataFactory.CreateShow(id: showId, tag: tag);
        _dbContext.Shows.Add(show);
        await _dbContext.SaveChangesAsync();

        _todaysShowsCache.SetCachedShows(new List<Show> { show });

        return (tag, show);
    }

    // Helper method to create a favorite tag for testing user preferences
    private async Task<FavouriteTag> CreateFavouriteTag(Tag tag)
    {
        var favouriteTag = new FavouriteTag
        {
            UserId = _testUserId,
            TagId = tag.Id
        };
        _dbContext.FavouriteTags.Add(favouriteTag);
        await _dbContext.SaveChangesAsync();
        return favouriteTag;
    }

    [Fact]
    public async Task SetGlobalRecommendation_ShouldCreateNewRecommendation()
    {
        // Arrange
        var (_, show) = await CreateTagAndShow(); // ignore the first value in here

        // Act
        await _globalGenerator.SetGlobalRecommendation();

        // Assert
        var recommendation = await _dbContext.GlobalRecommendations
            .Include(r => r.Show)
            .FirstOrDefaultAsync();

        Assert.NotNull(recommendation); // verifies that recommendation is created
        Assert.True(recommendation.Active); // verifies that recommendation is active
        Assert.Equal(show.ShowId, recommendation.ShowId); // verifies that recommendation is for the correct show
    }

    [Fact]
    public async Task SetGlobalRecommendation_ShouldDeactivatePreviousRecommendation()
    {
        // Arrange
        var tag = TestDataFactory.CreateTag();
        _dbContext.Tags.Add(tag);
        await _dbContext.SaveChangesAsync();

        var show1 = TestDataFactory.CreateShow(id: 1, tag: tag);
        var show2 = TestDataFactory.CreateShow(id: 2, tag: tag);
        _dbContext.Shows.AddRange(show1, show2);
        await _dbContext.SaveChangesAsync();

        _todaysShowsCache.SetCachedShows(new List<Show> { show1, show2 });

        // Create initial recommendation
        var initialRecommendation = new RecommendationGlobal
        {
            ShowId = show1.ShowId,
            Active = true,
            Added = DateTime.Now
        };
        _dbContext.GlobalRecommendations.Add(initialRecommendation);
        await _dbContext.SaveChangesAsync();

        // Act
        await _globalGenerator.SetGlobalRecommendation();

        // Assert
        var oldRecommendation = await _dbContext.GlobalRecommendations
            .FirstOrDefaultAsync(r => r.ShowId == show1.ShowId);
        var newRecommendation = await _dbContext.GlobalRecommendations
            .FirstOrDefaultAsync(r => r.ShowId == show2.ShowId);

        Assert.False(oldRecommendation.Active); // verifies that old recommendation is deactivated
        Assert.True(newRecommendation.Active); // verifies that new recommendation is active
    }

    [Fact]
    public async Task SetIndividualRecommendation_ShouldCreateNewRecommendation()
    {
        // Arrange
        var (tag, show) = await CreateTagAndShow();
        await CreateFavouriteTag(tag);

        // Act
        await _individualGenerator.SetIndividualRecommendation(_testUserId);

        // Assert
        var recommendation = await _dbContext.IndividualRecommendations
            .Include(r => r.Show)
            .FirstOrDefaultAsync(r => r.UserId == _testUserId);

        Assert.NotNull(recommendation); // verifies that recommendation is created
        Assert.Equal(show.ShowId, recommendation.ShowId); // verifies that recommendation is for the correct show
        Assert.Equal(DateTime.Today, recommendation.CreatedDate.Date); // verifies that recommendation is created today
    }

    [Fact]
    public async Task SetIndividualRecommendation_ShouldNotCreateDuplicateForSameDay()
    {
        // Arrange
        var (tag, show) = await CreateTagAndShow();
        await CreateFavouriteTag(tag);

        // Create initial recommendation
        var initialRecommendation = new RecommendationForUser
        {
            UserId = _testUserId,
            ShowId = show.ShowId,
            CreatedDate = DateTime.Today
        };
        _dbContext.IndividualRecommendations.Add(initialRecommendation);
        await _dbContext.SaveChangesAsync();

        // Act
        await _individualGenerator.SetIndividualRecommendation(_testUserId);

        // Assert
        var recommendations = await _dbContext.IndividualRecommendations
            .Where(r => r.UserId == _testUserId && r.CreatedDate.Date == DateTime.Today)
            .ToListAsync();

        Assert.Single(recommendations); // verifies that only one recommendation exists for today
    }

    [Fact]
    public async Task ClearGlobalRecommendationsHistory_ShouldRemoveAllRecommendations()
    {
        // Arrange
        var (_, show) = await CreateTagAndShow();

        var recommendation = new RecommendationGlobal
        {
            ShowId = show.ShowId,
            Active = true,
            Added = DateTime.Now
        };
        _dbContext.GlobalRecommendations.Add(recommendation);
        await _dbContext.SaveChangesAsync();

        // Act
        var allRecommendations = await _dbContext.GlobalRecommendations.ToListAsync();
        _dbContext.GlobalRecommendations.RemoveRange(allRecommendations);
        await _dbContext.SaveChangesAsync();

        // Assert
        var remainingRecommendations = await _dbContext.GlobalRecommendations.CountAsync();
        Assert.Equal(0, remainingRecommendations); // verifies that all recommendations are removed
    }

    [Fact]
    public async Task ClearIndividualRecommendationsHistoryForAllUsers_ShouldRemoveAllRecommendations()
    {
        // Arrange
        var (_, show) = await CreateTagAndShow(); 

        var recommendation = new RecommendationForUser
        {
            UserId = _testUserId,
            ShowId = show.ShowId,
            CreatedDate = DateTime.Today
        };
        _dbContext.IndividualRecommendations.Add(recommendation);
        await _dbContext.SaveChangesAsync();

        // Act
        var allRecommendations = await _dbContext.IndividualRecommendations.ToListAsync();
        _dbContext.IndividualRecommendations.RemoveRange(allRecommendations); // clear all
        await _dbContext.SaveChangesAsync();

        // Assert
        var remainingRecommendations = await _dbContext.IndividualRecommendations.CountAsync();
        Assert.Equal(0, remainingRecommendations); // verifies that all recommendations are removed
    }
}
