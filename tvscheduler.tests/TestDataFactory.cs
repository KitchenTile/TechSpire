using System.Net; // HttpResponseMessage
using System.Text; // strings to UTF-8
using System.Text.Json;
using tvscheduler.Models;

namespace tvscheduler.tests;

// Factory class for creating test data
public static class TestDataFactory
{
    public static HttpResponseMessage CreateTvProgramResponse() // testing API for TV program !live
    {
        return new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK, // 200
            Content = new StringContent( // Http res to Json
                @"{
                    ""programmes"": [
                        {
                            ""title"": ""Test Program"",
                            ""description"": ""Test Program Description"",
                            ""image"": ""https://example.com/image.jpg"",
                            ""startTime"": 1710345600,
                            ""duration"": 3600
                        }
                    ]
                }",
                Encoding.UTF8,
                "application/json"
            )
        };
    }

    public static HttpResponseMessage CreateTagResponse(string tagName = "Comedy") // testing API for return tag
    {
        var response = new // anonymous obj structure of the response to JSON
        {
            choices = new[]
            {
                new
                {
                    message = new
                    {
                        content = tagName
                    }
                }
            }
        };

        return new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                JsonSerializer.Serialize(response), // anonymous object to JSON
                Encoding.UTF8,
                "application/json"
            )
        };
    }

    public static Tag CreateTag(int id = 1, string name = "Comedy")
    {
        return new Tag
        {
            Id = id,
            Name = name
        };
    }

    public static Show CreateShow(int id = 1, string name = "Test Show", Tag? tag = null)
    {
        return new Show
        {
            ShowId = id,
            Name = name,
            ImageUrl = "https://test.com/image.jpg",
            Tag = tag
        };
    }

    public static Channel CreateChannel(int id = 1)
    {
        return new Channel
        {
            ChannelId = id,
            Name = "Test Channel",
            Description = "Test Channel Description",
            Lcn = id, // Logical Channel Number
            LogoUrl = "https://test.com/logo.png",
            Tstv = true // Time-shifted TV
        };
    }

    public static void SeedBasicTestData(AppDbContext dbContext)
    {
        // Add tag
        var tag = CreateTag();
        dbContext.Tags.Add(tag);

        // Add channel
        var channel = CreateChannel();
        dbContext.Channels.Add(channel);

        // Add show
        var show = CreateShow(tag: tag);
        dbContext.Shows.Add(show);

        // Add show event
        var showEvent = new ShowEvent
        {
            Id = 1,
            ChannelId = channel.ChannelId,
            Channel = channel,
            ShowId = show.ShowId,
            Show = show,
            TimeStart = 1710345600,
            Duration = 60,
            Description = "Test event description"
        };
        dbContext.ShowEvents.Add(showEvent);

        // Save changes
        dbContext.SaveChanges();
    }
}