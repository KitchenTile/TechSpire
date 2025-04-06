using System.Text.Json;
using tvscheduler.network_services;
using Xunit;

namespace tvscheduler.tests.Services;

public class TvApiTests : TestBase
{
    private readonly HttpClient _httpClient;
    
    public TvApiTests()
    {
        // Setup HTTP client with predefined mock response
        _httpClient = CreateMockHttpClient(TestDataFactory.CreateTvProgramResponse());
    }
    
    [Fact]
    public async Task FetchGuideData_ShouldReturnJsonElement() 
    {
        // Arrange alrdy done in constructor so no need
        
        // Act
        var result = await TvApi.FetchGuideData(_httpClient);
        
        // Assert
        Assert.NotNull(result); // verifies result !null
        Assert.True(result.Value.TryGetProperty("programmes", out var programmes)); // verifies programmes property exists
        Assert.Equal(1, programmes.GetArrayLength()); // verifies only 1 programme like 1 element
        
        var programme = programmes[0];
        Assert.True(programme.TryGetProperty("title", out var title));
        Assert.Equal("Test Program", title.GetString()); // verifies title is Test Program
        
        Assert.True(programme.TryGetProperty("description", out var description));
        Assert.Equal("Test Program Description", description.GetString()); // verifies description is Test Program Description
        
        Assert.True(programme.TryGetProperty("startTime", out var startTime)); 
        Assert.Equal(1710345600, startTime.GetInt32()); // verifies startTime is 1710345600
        
        Assert.True(programme.TryGetProperty("duration", out var duration));
        Assert.Equal(3600, duration.GetInt32()); // verifies duration is 3600
    }
    
    [Fact]
    public async Task FetchMultipleProgramData_WithValidChannelIds_ReturnsDictionary()
    {
        // Arrange
        var channelIds = new List<int> { 1, 2, 3 };
        
        // Act
        var result = await TvApi.FetchMultipleProgramData(_httpClient, channelIds);
        
        // Assert
        Assert.NotNull(result); // verifies result !null
        Assert.Equal(channelIds.Count, result.Count); // verifies result count is same as channelIds count
        
        // Each channel should have the same test data
        foreach (var channelId in channelIds)
        {
            Assert.True(result.ContainsKey(channelId)); // verifies result contains key for this channelId
            Assert.True(result[channelId].TryGetProperty("programmes", out var programmes)); // verifies programmes property exists
            Assert.Equal(1, programmes.GetArrayLength()); // verifies only 1 programme like 1 element
            
            var programme = programmes[0];
            Assert.True(programme.TryGetProperty("title", out var title)); // verifies title property exists
            Assert.Equal("Test Program", title.GetString()); // verifies title is Test Program
        }
    }
}