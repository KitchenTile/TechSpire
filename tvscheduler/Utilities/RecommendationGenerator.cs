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

public class RecomendationGeneratorGlobal : RecommendationGeneratorBase
{
    public RecomendationGeneratorGlobal(AppDbContext dbContext, TodaysShowsCache todaysShowsCache) : base(dbContext, todaysShowsCache) //dependency injection requires to explicitly pass the services through the constructors (cant just access with 'base')
    {
    }
    public async Task<Show> GetGlobalRecommendationAsync()
    {
        return await PickRandomShow(_todaysShowsCache.GetCachedShows());
    }
}

public class RecommendationGeneratorIndividual : RecommendationGeneratorBase
{
    public RecommendationGeneratorIndividual(AppDbContext dbContext, TodaysShowsCache todaysShowsCache) : base(dbContext, todaysShowsCache)
    {
    }
    public async Task<Show> GetIndividualRecommendationAsync(string userId)
    {
        throw new NotImplementedException("This method is not yet implemented.");
        //get all the shows for the given day
        //get user fav tags
        //  filter shows by tags
        // get and return a random show
    }
    
}