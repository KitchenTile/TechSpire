using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;

namespace tvscheduler.Utilities;

public class TodaysShowsCache
{
    private List<Show>? _cachedShows;
    private readonly AppDbContext _dbContext;

    public TodaysShowsCache(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Show> GetCachedShows()
    {
        return _cachedShows;
    }

    public void SetCachedShows(List<Show> shows)
    {
        _cachedShows = shows;
    }


    public async Task UpdateCachedShows()
    {
        var showEvents = await _dbContext.ShowEvents
            .Include(se => se.Show)
            .ToListAsync();
        
        // check if there are any events

        var earliestTimestamp = showEvents.Min(se => se.TimeStart);
        var latestTimestamp = showEvents.Max(se => se.TimeStart);

        var filteredShowEvents = showEvents
            .Where(se => se.TimeStart >= earliestTimestamp && se.TimeStart < latestTimestamp)
            .ToList();
        
        var shows = filteredShowEvents
            .Select(se => se.Show)
            .Distinct()
            .ToList();
        
        SetCachedShows(shows);
    }
}