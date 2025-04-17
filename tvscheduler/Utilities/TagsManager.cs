using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;
using tvscheduler.network_services;
namespace tvscheduler.Utilities;


/// Manages show tags using OpenAI for categorization
public class TagsManager
{
    private readonly AppDbContext _dbContext;
    private readonly HttpClient _httpClient;
    
    // Predefined list of valid tags for show categorization
    public readonly List<string> tagList = new List<string>
    {
        "Action", "Comedy", "Drama", "News", "Horror", "Sci-Fi", "Thriller", "Romance", "Documentary", "Animation", "Fantasy"
    };

    public TagsManager(AppDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }
    

    /// Assigns a tag to a show using OpenAI for categorization
    public async Task AssignTag(Show? show)
    {
        if (show != null)
        {
            var openAiHandler = new OpenAiHandler(_httpClient, tagList, show.Name);
            string gptresponse;

            // Ensure response is a valid tag from our list
            do
            {
                gptresponse = await openAiHandler.RequestTag();
            } while (!tagList.Contains(gptresponse));
            
            Console.WriteLine("TAG GENERATED FOR " + show.Name, " === ", gptresponse);
            
            // Get existing tag or create new one
            var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == gptresponse);
            if (tag == null)
            {
                tag = new Tag
                {
                    Name = gptresponse
                };
                _dbContext.Tags.Add(tag);
                await _dbContext.SaveChangesAsync();
            }
            show.Tag = tag;
        }
    }
    

    /// Scans database for shows without tags and assigns them
    public async Task CheckDatabaseForUntagged()
    {
        var shows = _dbContext.Shows.ToArray();

        foreach (var showObject in shows)
        {
            if (showObject.Tag == null)
            {
                await AssignTag(showObject);
            }
        }
        await _dbContext.SaveChangesAsync();
    }
    

    /// Reassigns tags for all shows in the database
    public async Task ReassignAllTags()
    {
        var shows = await _dbContext.Shows.ToArrayAsync();
        foreach (var showObject in shows)
        {
            Console.WriteLine("Reassigning " + showObject.Name);
            showObject.Tag = null;
            _dbContext.Entry(showObject).Property("TagId").IsModified = true;
        }  
        await _dbContext.SaveChangesAsync();
    }
    

    /// Removes tag associations from all shows
    public async Task DeleteTagIdsFromAllShows()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("UPDATE Shows SET TagId = NULL;");
        _dbContext.ChangeTracker.Clear();
    }


    /// Removes all tags from the database
    public async Task DeleteAllTags()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Tags;");
        _dbContext.ChangeTracker.Clear();
    }
}