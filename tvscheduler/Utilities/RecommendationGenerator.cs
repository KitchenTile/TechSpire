using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;

namespace tvscheduler.Utilities;


/// Base class for show recommendation generators
public abstract class RecommendationGeneratorBase
{
    protected readonly AppDbContext? _DbContext;
    protected readonly TodaysShowsCache _todaysShowsCache;

    protected RecommendationGeneratorBase(AppDbContext dbContext, TodaysShowsCache todaysShowsCache)
    {
        _DbContext = dbContext;
        _todaysShowsCache = todaysShowsCache;
    }
    
    /// <summary>
    /// Selects a random show from the list, optionally excluding recently recommended shows
    /// </summary>
    protected Show? PickRandomShow(List<Show> shows, List<int>? pastRecommendations = null)
    {
        var notRecentlyRecommended = pastRecommendations == null
            ? shows
            : shows.Where(x => !pastRecommendations.Contains(x.ShowId)).ToList();
        
        var random = new Random();
        return notRecentlyRecommended[random.Next(notRecentlyRecommended.Count)];
    }
}


/// Generates global recommendations for all users
public class RecommendationGeneratorGlobal : RecommendationGeneratorBase
{
    public RecommendationGeneratorGlobal(AppDbContext dbContext, TodaysShowsCache todaysShowsCache) : base(dbContext, todaysShowsCache)
    {
    }
    

    /// Sets a new global recommendation, deactivating the previous one
    public async Task SetGlobalRecommendation()
    {
        var pastRecommendations = await _DbContext.GlobalRecommendations
            .ToListAsync();
        var pastRecommendationsIds = pastRecommendations.Select(x => x.Id).ToList();
        
        // Deactivate current recommendation if exists
        var activeRecommendation = pastRecommendations.FirstOrDefault(r => r.Active);
        if (activeRecommendation != null)
        {
            activeRecommendation.Active = false;
            _DbContext.GlobalRecommendations.Update(activeRecommendation);
        }
        
        var randomShow = PickRandomShow(_todaysShowsCache.GetCachedShows(), pastRecommendationsIds);
        
        var newRecommendation = new RecommendationGlobal()
        {
            ShowId = randomShow.ShowId,
            Active = true,
            Added = DateTime.Now
        };
        
        _DbContext.GlobalRecommendations.Add(newRecommendation);
        await _DbContext.SaveChangesAsync();
    }
    

    /// Clears all global recommendations and resets the auto-increment counter
    public async Task ClearGlobalRecommendationsHistory()
    {
        var allRecommendations = await _DbContext.GlobalRecommendations.ToListAsync();
        _DbContext.GlobalRecommendations.RemoveRange(allRecommendations);
        
        await _DbContext.Database.ExecuteSqlRawAsync("ALTER TABLE GlobalRecommendations AUTO_INCREMENT = 1");
        
        await _DbContext.SaveChangesAsync();
    }
}


/// Generates personalized recommendations based on user preferences
public class RecommendationGeneratorIndividual : RecommendationGeneratorBase
{
    public RecommendationGeneratorIndividual(AppDbContext dbContext, TodaysShowsCache todaysShowsCache) : base(dbContext, todaysShowsCache)
    {
    }


    /// Generates a personalized recommendation for a user based on their favorite tags
    public async Task SetIndividualRecommendation(string userId)
    {
        // Check for existing recommendation within the last 3 days
        DateTime today = DateTime.Today;
        DateTime threeDaysAgo = DateTime.Today.AddDays(-3);
        var exsistingRecommendations = await _DbContext.IndividualRecommendations
            .Where(x => x.UserId == userId 
                        && x.CreatedDate <= today
                        && x.CreatedDate >= threeDaysAgo
                        )
            .Include(x => x.Show)
            .ThenInclude(x => x.Tag)
            .ToListAsync();
        
        var todaysRecommendation = exsistingRecommendations.FirstOrDefault(x => x.CreatedDate == today);

        if (todaysRecommendation != null)
        {
            return;
        }
            
        // Get user's favorite tags for personalized recommendations
        var userFavouriteTagIds = await _DbContext!.FavouriteTags
            .Where(ft => ft.UserId == userId)
            .Select(ft => ft.TagId)
            .ToListAsync();
        
        if (userFavouriteTagIds.Count == 0)
        {
            return;
        }

        List<Show> recommendedShows = _todaysShowsCache.GetTodaysShowsByTags(userFavouriteTagIds);
        
        if (recommendedShows.Count == 0)
        {
            return;
        }

        // Select a show that hasn't been recently recommended
        List<int> recentlyRecommendedShowIds = exsistingRecommendations.Select(x => x.Show.ShowId).ToList();
        var newRecommendation = PickRandomShow(recommendedShows, recentlyRecommendedShowIds);

        if (newRecommendation == null)
        {
            return;
        }

        _DbContext.IndividualRecommendations.Add(new RecommendationForUser()
        {
            UserId = userId,
            CreatedDate = DateTime.Today,
            ShowId = newRecommendation.ShowId
        });
        await _DbContext.SaveChangesAsync();
    }
    

    /// Clears all individual recommendations and resets the auto-increment counter
    public async Task ClearIndividualRecommendationsHistoryForAllUsers()
    {
        var allRecommendations = await _DbContext.IndividualRecommendations.ToListAsync();

        if (allRecommendations.Count == 0)
        {
            return;
        }
        
        _DbContext.IndividualRecommendations.RemoveRange(allRecommendations);
        await _DbContext.Database.ExecuteSqlRawAsync("ALTER TABLE IndividualRecommendations AUTO_INCREMENT = 1");
        await _DbContext.SaveChangesAsync();
    }
}