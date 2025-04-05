using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;

namespace tvscheduler.Utilities;

public abstract class RecommendationGeneratorBase
{
    protected readonly AppDbContext? _DbContext;
    protected readonly TodaysShowsCache _todaysShowsCache;

    protected RecommendationGeneratorBase(AppDbContext dbContext, TodaysShowsCache todaysShowsCache)
    {
        _DbContext = dbContext;
        _todaysShowsCache = todaysShowsCache;
    }
    
    protected Show? PickRandomShow(List<Show> shows, List<int>? pastRecommendations = null)
    {
        var notRecentlyRecommended = pastRecommendations == null
            ? shows
            : shows.Where(x => !pastRecommendations.Contains(x.ShowId)).ToList();
        
        var random = new Random();
        return notRecentlyRecommended[random.Next(notRecentlyRecommended.Count)];
    }
}



public class RecommendationGeneratorGlobal : RecommendationGeneratorBase
{
    public RecommendationGeneratorGlobal(AppDbContext dbContext, TodaysShowsCache todaysShowsCache) : base(dbContext, todaysShowsCache) //dependency injection requires to explicitly pass the services through the constructors (cant just access with 'base')
    {
    }
    
    // add method to check if theres a recommendation for today to use on app startup

    public async Task SetGlobalRecommendation()
    {
        var pastRecommendations = await _DbContext.GlobalRecommendations
            .ToListAsync();
        var pastRecommendationsIds = pastRecommendations.Select(x => x.Id).ToList();
        
        var activeRecommendation = pastRecommendations.FirstOrDefault(r => r.Active);
        if (activeRecommendation != null)
        {
            activeRecommendation.Active = false;
            _DbContext.GlobalRecommendations.Update(activeRecommendation);
        }
        
        var randomShow = PickRandomShow(_todaysShowsCache.GetCachedShows(), pastRecommendationsIds);
        
        // add the show as a current recommendation
        var newRecommendation = new RecommendationGlobal()
        {
            ShowId = randomShow.ShowId,
            Active = true,
            Added = DateTime.Now
        };
        
        _DbContext.GlobalRecommendations.Add(newRecommendation);
        await _DbContext.SaveChangesAsync();
    }
    
    public async Task ClearGlobalRecommendationsHistory()
    {
        var allRecommendations = await _DbContext.GlobalRecommendations.ToListAsync();
        _DbContext.GlobalRecommendations.RemoveRange(allRecommendations);
        
        await _DbContext.Database.ExecuteSqlRawAsync("ALTER TABLE RecommendationForUsers AUTO_INCREMENT = 1");
        
        await _DbContext.SaveChangesAsync();
    }
}



public class RecommendationGeneratorIndividual : RecommendationGeneratorBase
{
    public RecommendationGeneratorIndividual(AppDbContext dbContext, TodaysShowsCache todaysShowsCache) : base(dbContext, todaysShowsCache)
    {
    }
    public async Task SetIndividualRecommendation(string userId) //should be split into separate methods for funcions to follow a single purpose principle
    {
        //check if there already is a recommendation for today
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
            
        // if no exsisting recommendation - get users fav tags
        var userFavouriteTagIds = await _DbContext!.FavouriteTags
            .Where(ft => ft.UserId == userId)
            .Select(ft => ft.TagId)
            .ToListAsync();
        
        if (userFavouriteTagIds.Count == 0)
        {
            return; // No favorite tags to base recommendation on
            // how to handle this ? should the function be pure instead of making the db changes itself ?
        }
        

        List<Show> recommendedShows = _todaysShowsCache.GetTodaysShowsByTags(userFavouriteTagIds);
        
        if (recommendedShows.Count == 0)
        {
            return; // No shows match user's favorite tags today
        }

        // create a list of ids od recently recommended shows
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
        // if its first login today - show the recommendation splash before rest of the data is fetched??
    }
    
    public async Task ClearIndividualRecommendationsHistoryForAllUsers()
    {
        var allRecommendations = await _DbContext.IndividualRecommendations.ToListAsync();

        if (allRecommendations.Count == 0)
        {
            return;
        }
        
        _DbContext.IndividualRecommendations.RemoveRange(allRecommendations);
        await _DbContext.SaveChangesAsync();
    }
}