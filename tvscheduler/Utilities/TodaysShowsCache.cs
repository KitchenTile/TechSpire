using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;

namespace tvscheduler.Utilities;

public class TodaysShowsCache
{
    private List<Show>? _cachedShows;
    

    public List<Show> GetCachedShows()
    {
        return _cachedShows;
    }

    public void SetCachedShows(List<Show> shows)
    {
        _cachedShows = shows;
    }
}