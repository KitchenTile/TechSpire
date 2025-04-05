using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;
using System.Collections;

namespace tvscheduler.Utilities;

public class TodaysShowsCache
{
    private List<Show>? _cachedShows;
    private Dictionary<int, List<Show>> _ShowTagHashmap = new Dictionary<int, List<Show>>();
    

    public List<Show?> GetCachedShows()
    {
        return _cachedShows;
    }

    public Dictionary<int, List<Show>> GetShowTagHashmap()
    {
        return _ShowTagHashmap;
    }

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