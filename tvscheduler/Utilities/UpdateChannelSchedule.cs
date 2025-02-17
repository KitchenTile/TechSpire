using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;
using tvscheduler.network_services;

namespace tvscheduler.Utilities;

public class UpdateChannelSchedule
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext;
    private readonly TagsManager _tagsManager;

    public UpdateChannelSchedule(AppDbContext dbContext, HttpClient httpClient, TagsManager tagsManager)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _tagsManager = tagsManager;
    }
    
    public async Task UpdateDailySchedule()
    { 
        var channelIds = _dbContext.Channels.Select(c => c.ChannelId).ToList();

        var channelData = await TvApi.FetchMultipleProgramData(_httpClient, channelIds);
        
        foreach (var entry in channelData)
        {
            string formattedJson = JsonSerializer.Serialize(entry.Value, new JsonSerializerOptions { WriteIndented = true });

            
        }

        foreach (var kvp in channelData) // KeyValuePair<int, Channel>
        {
            int channelId = kvp.Key;
            var channel = kvp.Value; // Channel object
            
    
            foreach (var showEvent in channel[0].GetProperty("event").EnumerateArray())
            {
                Console.WriteLine(showEvent.GetProperty("name").GetString());
                
                var show = await _dbContext.Shows.FirstOrDefaultAsync(show => show.Name == showEvent.GetProperty("name").GetString());
                if (show == null)
                {
                    show = _dbContext.Shows.Add(new Show
                    {
                        Name = showEvent.GetProperty("name").GetString(),
                        Description = showEvent.GetProperty("description").GetString(),
                        ImageUrl = showEvent.GetProperty("image").GetString(),
                    }).Entity;
                    await _tagsManager.AssignTag(show);
                    await _dbContext.SaveChangesAsync();
                }

                _dbContext.ShowEvents.Add(new ShowEvent
                {
                    ChannelId = channelId,
                    ShowId = show.ShowId,
                    TimeStart = showEvent.GetProperty("startTime").GetInt32(),
                    Duration = showEvent.GetProperty("duration").GetInt32(),
                });
            }
            await _dbContext.SaveChangesAsync();
        }

    }
    
    // get the schedule
    
    // for show in schedule map to channel - showevent - show relation
}