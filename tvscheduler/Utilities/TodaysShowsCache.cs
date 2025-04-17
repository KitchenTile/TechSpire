using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;
using System.Collections;

namespace tvscheduler.Utilities;


/// In-memory cache for today's shows and their tag associations
public class TodaysShowsCache
{
    private List<Show>? _cachedShows;
    private Dictionary<int, List<Show>> _ShowTagHashmap = new Dictionary<int, List<Show>>();
    

    /// Returns all cached shows
    /// </summary>
    public List<Show?> GetCachedShows()
    {
        return _cachedShows;
    }

    /// Returns the tag-to-shows mapping
    public Dictionary<int, List<Show>> GetShowTagHashmap()
    {
        return _ShowTagHashmap;
    }


    /// Returns shows associated with the given tag IDs
    public List<Show> GetTodaysShowsByTags(List<int> tagIds)
    {
        List<Show> result = new List<Show>();

        foreach (var tagId in tagIds)
        {
            if (_ShowTagHashmap.ContainsKey(tagId))
            {
                result.AddRange(_ShowTagHashmap[tagId]);
            }
        }
        return result;
    }


    /// Updates the cache with new shows and rebuilds the tag mapping
    public void SetCachedShows(List<Show> shows)
    {
        _cachedShows = shows;

        _ShowTagHashmap.Clear();
        foreach(var Show in shows)
        {
            if (!_ShowTagHashmap.ContainsKey(Show.Tag.Id))
            {
                _ShowTagHashmap[Show.Tag.Id] = new List<Show>();
            }
            _ShowTagHashmap[Show.Tag.Id].Add(Show);
        }
    }
}