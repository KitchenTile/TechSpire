using Microsoft.EntityFrameworkCore;
using tvscheduler.network_services;
namespace tvscheduler.Utilities;

public class TagsManager
{
    private readonly AppDbContext _dbContext;
    private readonly HttpClient _httpClient;

    public TagsManager(AppDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }

    public async Task<string> getTagForShow(string showName)
    {
        //check in the database 
        var show = await _dbContext.Shows.FirstOrDefaultAsync(s => s.Name == showName);

        if (show == null)
        {
            List<string> tagList = new List<string>
            {
                "Action",
                "Comedy",
                "Drama",
                "News",
                "Horror",
                "Sci-Fi",
                "Thriller",
                "Romance",
                "Documentary",
                "Animation",
                "Fantasy"
            };
            var openAiHandler = new OpenAiHandler(_httpClient, tagList, showName);
            
            var gptresponse =  await openAiHandler.RequestTag();
            Console.WriteLine("HERES THE TAG FROM GPT: : : : " + gptresponse);
            return gptresponse;
        }
        return show.Name;
    }
}