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


var mockShow = new Show { Id = 1, Name = "Mock Show", StartTime = 1900, EndTime = 2000, ChannelId = 5 };
var mockChannel = new Channel { ChannelId = 01, Name = "Mock Channel", ChannelDescription = "mock desc", ShowList = [mockShow] };


mockChannel.DisplayChannel();

// test endpoint
app.MapGet("/", () =>
{
    return Results.Ok(mockChannel);
});

app.Run();