namespace tvscheduler.Utilities;

public class HangfireJobs
{

        private readonly IServiceScopeFactory _scopeFactory;

        public HangfireJobs(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        
        public async Task UpdateChannelScheduleJob()
        {
            using var scope = _scopeFactory.CreateScope();
            var updateService = scope.ServiceProvider.GetRequiredService<UpdateChannelSchedule>();
            await updateService.UpdateDailySchedule();
        }
        public async Task CheckDatabaseForUntagged()
        {
            using var scope = _scopeFactory.CreateScope();
            var tagsManager = scope.ServiceProvider.GetRequiredService<TagsManager>();
            await tagsManager.CheckDatabaseForUntagged();
        }

        public async Task ReassignAllTags()
        {
            using var scope = _scopeFactory.CreateScope();
            var tagsManager = scope.ServiceProvider.GetRequiredService<TagsManager>();
            await tagsManager.ReassignAllTags();
        }

        public async Task DeleteTagIdsFromAllShows()
        {
            using var scope = _scopeFactory.CreateScope();
            var tagsManager = scope.ServiceProvider.GetRequiredService<TagsManager>();
            await tagsManager.DeleteTagIdsFromAllShows();
        }

        public async Task DeleteAllTags()
        {
            using var scope = _scopeFactory.CreateScope();
            var tagsManager = scope.ServiceProvider.GetRequiredService<TagsManager>();
            await tagsManager.DeleteAllTags();
        }

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
    }