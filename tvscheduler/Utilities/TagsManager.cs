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
            var openAiHandler = new OpenAiHandler(_httpClient);

            return openAiHandler.RequestTag(showName);
        }
        return show.Name;
    }
}