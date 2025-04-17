using Microsoft.EntityFrameworkCore;

namespace tvscheduler.Utilities;


/// Updates the in-memory cache of today's shows
public class TodaysShowsCacheUpdate
{
    protected readonly AppDbContext _DbContext;
    protected readonly TodaysShowsCache _ShowsCache;
    
    public TodaysShowsCacheUpdate(AppDbContext dbContext, TodaysShowsCache showsCache)
    {
        _DbContext = dbContext;
        _ShowsCache = showsCache;
    }
    

    /// Updates the cache with shows from the current time period
    public async Task UpdateCachedShows()
    {
        // Load show events with related shows and tags
        var showEvents = await _DbContext.ShowEvents
            .Include(se => se.Show)
            .ThenInclude(s => s.Tag)
            .ToListAsync();
        
        // Get time range for current shows
        var earliestTimestamp = showEvents.Min(se => se.TimeStart);
        var latestTimestamp = showEvents.Max(se => se.TimeStart);

        // Filter shows within the current time period
        var filteredShowEvents = showEvents
            .Where(se => se.TimeStart >= earliestTimestamp && se.TimeStart < latestTimestamp)
            .ToList();
        
        // Get unique shows and update cache
        var shows = filteredShowEvents
            .Select(se => se.Show)
            .Distinct()
            .ToList();
        
        _ShowsCache.SetCachedShows(shows);
    }
}

