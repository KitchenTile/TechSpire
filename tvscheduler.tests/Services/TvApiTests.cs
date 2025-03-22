using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using tvscheduler.network_services;
using Xunit;

namespace tvscheduler.tests.Services;

public class TvApiTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    
    public TvApiTests()
    {
        // Setup mock HTTP handler
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        
        // Setup mock response for TV API
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), // Matching any HttpRequestMessage.
                ItExpr.IsAny<CancellationToken>() // Matching any CancellationToken.
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK, // 200 OK
                Content = new StringContent( 
                    @"{""programmes"": [
                        {
                            ""title"": ""Test Program"",
                            ""description"": ""Test Program Description"",
                            ""image"": ""https://example.com/image.jpg"",
                            ""startTime"": 1710345600,
                            ""duration"": 3600
                        }
                    ]}", 
                    Encoding.UTF8, 
                    "application/json"
                )
            });
        
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
    }
    
    [Fact]
    public async Task FetchMultipleProgramData_WithValidChannelIds_ReturnsDictionary()
    {
        // Arrange
        var channelIds = new List<int> { 1, 2, 3 };
        
        // Act
        var result = await TvApi.FetchMultipleProgramData(_httpClient, channelIds); // call back and store the results
        
        // Assert
        Assert.NotNull(result);
        // The mock will return the same response for each channel
        Assert.Equal(channelIds.Count, result.Count); // Asserting that the count of the result matches the count of the channel IDs
    }
}