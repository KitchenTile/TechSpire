using System.Text;
using System.Text.Json;

namespace tvscheduler.network_services;

public class OpenAiHandler
{
    public readonly HttpClient _httpClient;
    public readonly List<string> tags;
    public readonly string show;




    public OpenAiHandler(HttpClient httpClient, List<string> tagList, string showname)
    {
        _httpClient = httpClient;
        tags = tagList;
        show = showname;

    }


    public async Task<string?> RequestTag()
    {
        try
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = ProomptGenerator(tags) },  // Lowercase "system"
                    new { role = "user", content = show }
                }
            };

            string jsonRequest = JsonSerializer.Serialize(requestBody);
            Console.WriteLine("Sending request: " + jsonRequest);  // Debugging

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");

            request.Headers.Add("Authorization", "Bearer"); 

            request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Raw response: " + jsonResponse);  // Debugging

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}, {jsonResponse}");  // Show error instead of throwing immediately
                return null;
            }

            using JsonDocument doc = JsonDocument.Parse(jsonResponse);
            string? result = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            Console.WriteLine("ANSWER FROM GPT: " + result);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in RequestTag(): " + ex.Message);
            return null;
        }
    }

    private string ProomptGenerator(List<string> tags)
    {
        string proompt = "Please respond with a single string only which will be one of those tags which best fits the given tv show or movie: ";

        string tagString = string.Join(", ", tags);
        // add list of tags

        return proompt + tagString;
    }

}