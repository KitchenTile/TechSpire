using Microsoft.EntityFrameworkCore;

namespace tvscheduler.Utilities;

public class TodaysShowsCacheUpdate
{
    protected readonly AppDbContext _DbContext;
    protected readonly TodaysShowsCache _ShowsCache;
    
    public TodaysShowsCacheUpdate(AppDbContext dbContext, TodaysShowsCache showsCache)
    {
        _DbContext = dbContext;
        _ShowsCache = showsCache;
    }
    
    public async Task UpdateCachedShows()
    {
        var showEvents = await _DbContext.ShowEvents
            .Include(se => se.Show)
            .ThenInclude(s => s.Tag)
            .ToListAsync();
        
        // check if there are any events !!

        var earliestTimestamp = showEvents.Min(se => se.TimeStart);
        var latestTimestamp = showEvents.Max(se => se.TimeStart);

        var filteredShowEvents = showEvents
            .Where(se => se.TimeStart >= earliestTimestamp && se.TimeStart < latestTimestamp)
            .ToList();
        
        var shows = filteredShowEvents
            .Select(se => se.Show)
            .Distinct()
            .ToList();
        
        _ShowsCache.SetCachedShows(shows);
    }
}

