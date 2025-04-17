namespace tvscheduler.Utilities;

/// Defines background jobs for the TV scheduler application
public class HangfireJobs
{
    private readonly IServiceScopeFactory _scopeFactory;

    public HangfireJobs(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    /// Updates channel schedules for the next two days
    public async Task UpdateChannelScheduleJob()
    {
        using var scope = _scopeFactory.CreateScope();
        var updateService = scope.ServiceProvider.GetRequiredService<UpdateChannelSchedule>();
        await updateService.UpdateDailySchedule();
    }

    /// Scans database for shows without tags and attempts to assign them
    public async Task CheckDatabaseForUntagged()
    {
        using var scope = _scopeFactory.CreateScope();
        var tagsManager = scope.ServiceProvider.GetRequiredService<TagsManager>();
        await tagsManager.CheckDatabaseForUntagged();
    }

    /// Reassigns tags to all shows in the database
    public async Task ReassignAllTags()
    {
        using var scope = _scopeFactory.CreateScope();
        var tagsManager = scope.ServiceProvider.GetRequiredService<TagsManager>();
        await tagsManager.ReassignAllTags();
    }

    /// Removes tag references from all shows
    public async Task DeleteTagIdsFromAllShows()
    {
        using var scope = _scopeFactory.CreateScope();
        var tagsManager = scope.ServiceProvider.GetRequiredService<TagsManager>();
        await tagsManager.DeleteTagIdsFromAllShows();
    }

    /// Removes all tags from the database
    public async Task DeleteAllTags()
    {
        using var scope = _scopeFactory.CreateScope();
        var tagsManager = scope.ServiceProvider.GetRequiredService<TagsManager>();
        await tagsManager.DeleteAllTags();
    }

    /// Updates the cache of today's shows
    public async Task UpdateTodaysShowsCache()
    {
        using var scope = _scopeFactory.CreateScope();
        var cacheUpdater = scope.ServiceProvider.GetRequiredService<TodaysShowsCacheUpdate>();
        await cacheUpdater.UpdateCachedShows();
    }

        public async Task UpdateGlobalRecommendation()
        {
            using var scope = _scopeFactory.CreateScope();
            var globalRecommendationUpdater = scope.ServiceProvider.GetRequiredService<RecommendationGeneratorGlobal>();
            await globalRecommendationUpdater.SetGlobalRecommendation();
        }
        
        public async Task ClearGlobalRecommendationsHistory()
        {
            using var scope = _scopeFactory.CreateScope();
            var globalRecommendationManager = scope.ServiceProvider.GetRequiredService<RecommendationGeneratorGlobal>();
            await globalRecommendationManager.ClearGlobalRecommendationsHistory();
        }

    /// Clears individual recommendation history for all users
    public async Task ClearIndividualRecommendationsHistory()
    {
        using var scope = _scopeFactory.CreateScope();
        var individualRecommendationManager = scope.ServiceProvider.GetRequiredService<RecommendationGeneratorIndividual>();
        await individualRecommendationManager.ClearIndividualRecommendationsHistoryForAllUsers();
    }

    /// Processes and resizes images for all shows
    public async Task ProcessShowImages()
    {
        using var scope = _scopeFactory.CreateScope();
        var imageManager = scope.ServiceProvider.GetRequiredService<ImageManager>();
        await imageManager.ProcessAllShowImages();
    }

    /// Removes all resized show images
    public async Task DeleteAllResizedImages()
    {
        using var scope = _scopeFactory.CreateScope();
        var imageManager = scope.ServiceProvider.GetRequiredService<ImageManager>();
        await imageManager.DeleteAllResizedImages();
    }
}