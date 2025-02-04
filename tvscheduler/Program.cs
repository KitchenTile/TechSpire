using System.Net.Http;
using System.Text.Json;
using tvscheduler;

var builder = WebApplication.CreateBuilder(args);

//add cors policies
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
var app = builder.Build();
app.UseCors("AllowFrontend");


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

// New func to fetch multiple channels' data at once using a list of ids
async Task<Dictionary<int, JsonElement>> FetchMultipleProgramData(HttpClient httpClient, List<int> channelIds)
{
    var fetchTasks = channelIds.Select(async channelId =>
    {
        var response = await httpClient.GetAsync($"https://www.freesat.co.uk/tv-guide/api/0?channel={channelId}");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var programData = JsonSerializer.Deserialize<JsonElement>(responseBody);

        //returns a kvp of {id -> data}
        return new KeyValuePair<int, JsonElement>(channelId, programData);
    });
=======
async Task<JsonElement> FetchProgramData(HttpClient httpClient, int channelId = 560)
{
    var response = await httpClient.GetAsync($"https://www.freesat.co.uk/tv-guide/api/0?channel={channelId}");
    response.EnsureSuccessStatusCode();

    var responseBody = await response.Content.ReadAsStringAsync();
    var programData = JsonSerializer.Deserialize<JsonElement>(responseBody);

    return programData;
}

    var results = await Task.WhenAll(fetchTasks);

    return results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
}

// entry endpoint with external API calls
app.MapGet("/", async (HttpClient httpClient) =>
{
    var guideData = await FetchGuideData(httpClient);
    // var programData = await FetchProgramData(httpClient);
    var channelIds = new List<int> { 560, 700, 10005, 1540, 1547 }; //first 5 ids in guideData - needs change
    var programData = await FetchMultipleProgramData(httpClient, channelIds);

    return Results.Ok(new
    {
        guideData,
        channelIds,
        programData,
        mySchedule
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

    var showObject = channelObject?.ShowList.FirstOrDefault(s => s.EvtID == id);

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
    var showObject = mySchedule.FirstOrDefault(s => s.EvtID == id);


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