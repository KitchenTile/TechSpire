using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;
using tvscheduler.network_services;

namespace tvscheduler.Utilities;

public class UpdateChannelSchedule
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext;

    public UpdateChannelSchedule(AppDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }
    
    public async Task UpdateDailySchedule()
    {
        //List<int> channelIds = new List<int>();
        var channelIds = new List<int> { 10005, 1540, 1547 };
        var channels = _dbContext.Channels;

     //   foreach (var channel in channels)
     //   {
     //       channelIds.Add(channel.ChannelId);
     //   }
        Console.WriteLine("Channel IDs:" + string.Join(", ", channelIds));
        
        if (_httpClient == null)
        {
            Console.WriteLine("Error: httpClient is null.");
            return;
        }

        var channelData = await TvApi.FetchMultipleProgramData(_httpClient, channelIds);
        
        foreach (var entry in channelData)
        {
            string formattedJson = JsonSerializer.Serialize(entry.Value, new JsonSerializerOptions { WriteIndented = true });
            //Console.WriteLine($"Channel {entry.Key}:\n{formattedJson}");
            
        }
        //Console.WriteLine("Channel Data:" + channelData[1540]);

        foreach (var kvp in channelData) // kvp = KeyValuePair<int, Channel>
        {
            int channelId = kvp.Key;
            var channel = kvp.Value; // This is the Channel object

            Console.WriteLine($"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Channel ID: {channelId}");
    
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