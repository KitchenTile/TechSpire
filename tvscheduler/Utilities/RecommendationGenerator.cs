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
    
    protected async Task<Show?> PickRandomShow(List<Show?> shows)
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
        
        Show? randomShow;
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
    public async Task<Show?> SetIndividualRecommendation(string userId) //should be split into separate methods for funcions to follow a single purpose principle
    {
        //check if there already is a recommendation for today
        DateTime today = DateTime.Today;
        var exsistingRecommendation = await _DbContext.IndividualRecommendations
            .Where(x => x.UserId == userId 
                        && x.CreatedDate >= today
                        )
            .Include(x => x.Show)
            .FirstOrDefaultAsync();

        if (exsistingRecommendation != null) 
        {
            return await _DbContext.Shows.FirstOrDefaultAsync(s => s.ShowId == exsistingRecommendation.Show.ShowId);
        }
            
        // if no exsisting recommendation - get users fav tags
        var userFavouriteTags = await _DbContext!.FavouriteTags
            .Where(ft => ft.UserId == userId)
            .Select(ft => ft.TagId)
            .ToListAsync();
        
        if (userFavouriteTags.Count == 0)
        {
            return null; // No favorite tags to base recommendation on
        }
        
        var todaysShows = _todaysShowsCache.GetCachedShows();

        
        var recommendedShows = todaysShows
            .Where(show => show.Tag != null && userFavouriteTags.Contains(show.Tag.Id))
            .ToList();
        
        if (recommendedShows.Count == 0)
        {
            return null; // No shows match user's favorite tags today
        }

        var newRecommendation = await PickRandomShow(recommendedShows);

        _DbContext.IndividualRecommendations.Add(new RecommendationForUser()
        {
            UserId = userId,
            CreatedDate = DateTime.Now,
            ShowId = newRecommendation.ShowId
        });
        await _DbContext.SaveChangesAsync();
        
        return newRecommendation;
    } 
}           // if its first login today - show the recommendation splash before rest of the data is fetched??