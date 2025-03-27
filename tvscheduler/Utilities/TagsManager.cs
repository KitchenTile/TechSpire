using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;
using tvscheduler.network_services;
namespace tvscheduler.Utilities;

public class TagsManager
{
    private readonly AppDbContext _dbContext;
    private readonly HttpClient _httpClient;
    public readonly List<string> tagList = new List<string>
    {
        "Action", "Comedy", "Drama", "News", "Horror", "Sci-Fi", "Thriller", "Romance", "Documentary", "Animation", "Fantasy"
    };

    public TagsManager(AppDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }
    
    // assign tag() to separate checking if the show exsists from getTagForShow()
    public async Task AssignTag(Show? show)
    {

        if (show != null)
        {
            
            var openAiHandler = new OpenAiHandler(_httpClient, tagList, show.Name);
            string gptresponse;

            //           !! verify gpt answer!!! is the response in the provided tag list ?
            do
            {
                gptresponse = await openAiHandler.RequestTag();

            } while ( !tagList.Contains(gptresponse) );
            
        
            
            Console.WriteLine("TAG GENERATED FOR " + show.Name, " === ", gptresponse);
            
            //get or create tag,
            var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == gptresponse);
            if (tag == null)
            {
                tag = new Tag
                {
                    Name = gptresponse
                };
                _dbContext.Tags.Add(tag);
                await _dbContext.SaveChangesAsync();
                // no need to run save because if will be run in the show creator after this function returns
                //      - well not really because then tag == null was always true on line 48
            }
            show.Tag = tag;  // assign tag to the show
        }
    }
    
    // check all the shows in a db for a missing tag
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
    
    // reassign tags for all shows after changing the tags list
    // for each show > delete tag > assign new tag
    public async Task ReassignAllTags()
    {
        var shows = await _dbContext.Shows.ToArrayAsync();
        foreach (var showObject in shows)
        {
            Console.WriteLine("Reassigning " + showObject.Name);
            //remove tag
            showObject.Tag = null;
            _dbContext.Entry(showObject).Property("TagId").IsModified = true;  // explicitly mark as updated
            //assign tags
            // await AssignTag(showObject);`
            
        }  
        await _dbContext.SaveChangesAsync();
    }
    
    // should i rewrite it in a more functional way ? 
    // create an iterator which takes a callback (like remove, find tag etc?
    
    // removes link from Show to Tag for all the shows in the DB 
    public async Task DeleteTagIdsFromAllShows()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("UPDATE Shows SET TagId = NULL;");
        _dbContext.ChangeTracker.Clear();
    }

    // Clears the Tags table!
    public async Task DeleteAllTags()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Tags;");
        _dbContext.ChangeTracker.Clear();
    }
}