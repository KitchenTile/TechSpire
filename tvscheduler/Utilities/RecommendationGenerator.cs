using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;

namespace tvscheduler.Utilities;

public abstract class RecommendationGeneratorBase
{
    protected readonly AppDbContext? _DbContext;

    protected RecommendationGeneratorBase(AppDbContext? dbContext)
    {
        _DbContext = dbContext;
    }

    protected async Task<List<Show>> GetAllShowsAsync() // change to only pick the shows based on todays showEvents
    {
        return await _DbContext!.Shows.ToListAsync();
    }

    protected async Task<Show> PickRandomShow(List<Show> shows)
    {
        var random = new Random();
        return shows[random.Next(shows.Count)];
    }
}

public class RecomendationGeneratorGlobal : RecommendationGeneratorBase
{
    public RecomendationGeneratorGlobal(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Show> GetGlobalRecommendationAsync()
    {
        var shows = await GetAllShowsAsync();
        return await PickRandomShow(shows);
    }
}

public class RecommendationGeneratorIndividual : RecommendationGeneratorBase
{
    public RecommendationGeneratorIndividual(AppDbContext dbContext) : base(dbContext)
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