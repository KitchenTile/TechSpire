using tvscheduler.Models;

namespace tvscheduler.Utilities;

public class TodaysShowsCache
{
    private List<Show>? _cachedShows;

    public List<Show> GetCachedShows()
    {
        return _cachedShows;
    }

    public void UpdateCachedShows(List<Show> shows)
    {
        _cachedShows = shows;
    }
}