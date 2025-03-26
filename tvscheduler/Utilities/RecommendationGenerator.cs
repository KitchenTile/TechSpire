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
    
    protected async Task<Show> PickRandomShow(List<Show> shows)
    {
        var random = new Random();
        return shows[random.Next(shows.Count)];
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
        
        Show randomShow;
        do
        {
            randomShow = await PickRandomShow(_todaysShowsCache.GetCachedShows());
        } while (pastRecommendationsIds.Contains(randomShow.ShowId));
        
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
}



public class RecommendationGeneratorIndividual : RecommendationGeneratorBase
{
    public RecommendationGeneratorIndividual(AppDbContext dbContext, TodaysShowsCache todaysShowsCache) : base(dbContext, todaysShowsCache)
    {
    }
    public async Task<Show> GetIndividualRecommendation(string userId)
    {
        var userFavouriteTags = await _DbContext!.FavouriteTags
            .Where(ft => ft.UserId == userId)
            .Select(ft => ft.TagId)
            .ToListAsync();
        
        var todaysShows = _todaysShowsCache.GetCachedShows();
        
        
        var recommendedShows = todaysShows
            .Where(show => show.Tag != null && userFavouriteTags.Contains(show.Tag.Id))
            .ToList();
        
        return await PickRandomShow(recommendedShows); 
        // save to db to be implemented
    }
}