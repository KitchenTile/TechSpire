using tvscheduler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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



// entry endpoint
app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        channels,
        mySchedule
    });
});



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