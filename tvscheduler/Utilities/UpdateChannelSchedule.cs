using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;
using tvscheduler.network_services;

namespace tvscheduler.Utilities;


/// Updates channel schedules by fetching and processing TV guide data
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
    

    /// Fetches and updates the schedule for all channels
    public async Task UpdateDailySchedule()
    { 
        // Get all channel IDs from the database
        var channelIds = _dbContext.Channels.Select(c => c.ChannelId).ToList();

        // Fetch program data for all channels
        var channelData = await TvApi.FetchMultipleProgramData(_httpClient, channelIds);
        
        foreach (var entry in channelData)
        {
            string formattedJson = JsonSerializer.Serialize(entry.Value, new JsonSerializerOptions { WriteIndented = true });
        }

        // Process each channel's schedule
        foreach (var kvp in channelData)
        {
            int channelId = kvp.Key;
            var channel = kvp.Value;
            
            // Process each show event in the schedule
            foreach (var showEvent in channel[0].GetProperty("event").EnumerateArray())
            {
                Console.WriteLine(showEvent.GetProperty("name").GetString());
                
                // Find or create show in database
                var show = await _dbContext.Shows.FirstOrDefaultAsync(show => show.Name == showEvent.GetProperty("name").GetString());
                if (show == null)
                {
                    show = _dbContext.Shows.Add(new Show
                    {
                        Name = showEvent.GetProperty("name").GetString(),
                        ImageUrl = showEvent.GetProperty("image").GetString(),
                    }).Entity;
                    await _tagsManager.AssignTag(show);
                    await _dbContext.SaveChangesAsync();
                }

                // Add show event to schedule
                _dbContext.ShowEvents.Add(new ShowEvent
                {
                    ChannelId = channelId,
                    ShowId = show.ShowId,
                    Description = showEvent.GetProperty("description").GetString(),
                    TimeStart = showEvent.GetProperty("startTime").GetInt32(),
                    Duration = showEvent.GetProperty("duration").GetInt32(),
                });
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}