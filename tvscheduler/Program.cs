using tvscheduler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var mockChannel = new Channel { ChannelId = 1, Name = "Mock Channel", ChannelDescription = "mock desc", ShowList = new List<Show>() };
var mockChannel2 = new Channel { ChannelId = 2, Name = "Mock Channel 2", ChannelDescription = "mock desc", ShowList = new List<Show>() };

var mockShow = new Show { Id = 1, Name = "Mock Show", StartTime = 1900, EndTime = 2000, ChannelId = new List<Channel> { mockChannel, mockChannel2 } };
var mockShow2 = new Show { Id = 2, Name = "Mock Show2", StartTime = 1600, EndTime = 1900, ChannelId = new List<Channel> { mockChannel } };
var mockShow3 = new Show { Id = 3, Name = "Mock Show3", StartTime = 2000, EndTime = 2100, ChannelId = new List<Channel> { mockChannel, mockChannel2 } };

mockChannel.ShowList = new List<Show> { mockShow, mockShow2, mockShow3 };
mockChannel2.ShowList = new List<Show> { mockShow, mockShow3 };

mockChannel.DisplayChannel();
Console.WriteLine("------------");
mockChannel2.DisplayChannel();
// test endpoint
app.MapGet("/", () =>
{
    return Results.Ok(mockChannel);
});

app.Run();