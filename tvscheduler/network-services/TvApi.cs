using System.Text.Json;

namespace tvscheduler.network_services;

public class TvApi
{
    public static async Task<JsonElement?> FetchGuideData(HttpClient httpClient)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.freesat.co.uk/tv-guide/api");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            request.Headers.Add("Accept", "application/json");

            var response = await httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode) // ✅ Check status code before proceeding
            {
                Console.WriteLine($"API request failed: {response.StatusCode}");
                return null; // ✅ Return null instead of crashing
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var guideData = JsonSerializer.Deserialize<JsonElement>(responseBody);

            return guideData;
        }
        catch (JsonException jsonEx) // ✅ Handle JSON parsing errors
        {
            Console.WriteLine($"JSON parsing error: {jsonEx.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching guide data: {ex.Message}");
            return null;
        }
    }





    public static async Task<Dictionary<int, JsonElement>> FetchMultipleProgramData(HttpClient httpClient,
        List<int> channelIds)
    {
        var results = new Dictionary<int, JsonElement>();

        foreach (var channelId in channelIds)
        {
            try
            {
                var response =
                    await httpClient.GetAsync($"https://www.freesat.co.uk/tv-guide/api/0?channel={channelId}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(
                        $"Failed to fetch data for channel {channelId}, status code: {response.StatusCode}");
                    continue; // ✅ Skip this channel if it fails
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var programData = JsonSerializer.Deserialize<JsonElement>(responseBody);

                results[channelId] = programData;
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON parsing error for channel {channelId}: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching program data for channel {channelId}: {ex.Message}");
            }
        }

        return results; // ✅ Returns only successfully fetched data
    }



}