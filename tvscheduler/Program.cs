using System.Net.Http;
using System.Text.Json;
using tvscheduler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();


var scheduleRoute = app.MapGroup("schedule");
var mySchedule = new List<Show>();



// creating mock data
var mockChannel1 = ChannelGenerator.GenerateMockChannel(
        channelId: 1,
        scheduleStartTime: DateTime.Now,
        showCount: 5,
        showDuration: 5
        );
var mockChannel2 = ChannelGenerator.GenerateMockChannel(
        channelId: 2,
        scheduleStartTime: DateTime.Now,
        showCount: 5,
        showDuration: 5
);

var channels = new List<Channel>
{
    mockChannel1,
    mockChannel2
};

// Fetch TV Guide data from the API
async Task<JsonElement> FetchGuideData(HttpClient httpClient)
{
    var response = await httpClient.GetAsync("https://www.freesat.co.uk/tv-guide/api");
    response.EnsureSuccessStatusCode();

    var responseBody = await response.Content.ReadAsStringAsync();
    var guideData = JsonSerializer.Deserialize<JsonElement>(responseBody);

    return guideData;
}

// Fetch TV Guide data for a specific channel
async Task<JsonElement> FetchProgramData(HttpClient httpClient, int channelId = 560)
{
    var response = await httpClient.GetAsync($"https://www.freesat.co.uk/tv-guide/api/0?channel={channelId}");
    response.EnsureSuccessStatusCode();

    var responseBody = await response.Content.ReadAsStringAsync();
    var programData = JsonSerializer.Deserialize<JsonElement>(responseBody);

    return programData;
}


// entry endpoint with external API calls
app.MapGet("/", async (HttpClient httpClient) =>
{
    var guideData = await FetchGuideData(httpClient);
    var programData = await FetchProgramData(httpClient);

    return Results.Ok(new
    {
        guideData,
        programData
    });
});

/*
// entry endpoint
app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        channels,
        mySchedule
    });
});
*/


// add to schedule
// ex: /schedule/add?id=500
scheduleRoute.MapGet("add", (int id, int channel) =>
{
    var channelObject = channels.FirstOrDefault(c => c.ChannelId == channel);
    var showObject = channelObject?.ShowList.FirstOrDefault(s => s.evtID == id);

    if (showObject != null)
    {
        mySchedule.Add(showObject);
    }
    else
    {
        return Results.NotFound();
    }

    return Results.Ok("ok");
});



// remove from the schedule
scheduleRoute.MapGet("remove", (int id) =>
{
    var showObject = mySchedule.FirstOrDefault(s => s.evtID == id);

    if (showObject != null)
    {
        mySchedule.Remove(showObject);
    }
    else
    {
        return Results.NotFound();
    }
    
    return Results.Ok("ok");
});



app.Run();