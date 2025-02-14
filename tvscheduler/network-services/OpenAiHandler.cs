namespace tvscheduler.network_services;

public class OpenAiHandler
{
    public readonly HttpClient _httpClient;
    


    public OpenAiHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public string RequestTag(string tag)
    {
        
    }

    private string ConnectionHandler(string proompt)
    {
        
    }

    private string ProomptGenerator(string tag)
    {
        string proompt = "Please respond with a single string only which will be one of provided tags.";
        
        
        // add list of tags
        
        // add the name of the show to be tagged
        
    }
    
}