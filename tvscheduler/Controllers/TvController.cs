using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using tvscheduler.network_services;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;
using tvscheduler.Utilities;

namespace tvscheduler.Controllers;


[Microsoft.AspNetCore.Components.Route("")]
[ApiController]
public class TvController : ControllerBase
{

    private readonly AppDbContext _DbContext;
    private readonly HttpClient _httpClient;
    private readonly UserManager<User> _userManager;
    private readonly TagsManager _tagsManager;
    private readonly TodaysShowsCache _todaysShowsCache;

    public TvController(HttpClient httpClient, AppDbContext dbContext, UserManager<User> userManager, TagsManager tagsManager, TodaysShowsCache todaysShowsCache)
    {
        _httpClient = httpClient;
        _DbContext = dbContext;
        _userManager = userManager;
        _tagsManager = tagsManager;
        _todaysShowsCache = todaysShowsCache;
    }
    
    
    // ENTRY ENDPOINT
    [HttpGet("/main")]
    public async Task<IActionResult> EntryEndpoint()
    {
        // Check if the user is authenticated
        if (HttpContext.User?.Identity?.IsAuthenticated ?? false)
        {
            // Use await instead of .Result to avoid blocking
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            
                 
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var userSchedule = await _DbContext.ScheduleEvents
                .Include(a=>a.ShowEvent)
                .ThenInclude(s=>s.Show)
                .Where(s => s.UserId == user.Id)
                .ToListAsync();
            
            var favouriteTags = await _DbContext.FavouriteTags
                .Where(ft => ft.UserId == user.Id)
                .Select(ft => new FavouriteTagDTO
                {
                    Id = ft.Id,
                    TagId = ft.TagId,
                    TagName = ft.Tag.Name
                    
                })
                .ToListAsync();
            
            var channelsData = await _DbContext.Channels
                .Include(c => c.ShowEvents)
                .ToListAsync();

        

            var response = userSchedule.Select(se => new MainEndpointResponseDto
            {
                UserScheduleItemId = se.Id,
                ShowEvent = new ShowEventDto
                {
                    ShowEventId = se.ShowEvent.Id,
                    ChannelId = se.ShowEvent.ChannelId,
                    Description = se.ShowEvent.Description,
                    ShowId = se.ShowEvent.ShowId,
                    TimeStart = se.ShowEvent.TimeStart,
                    Duration = se.ShowEvent.Duration
                }
            }).ToList();
            
            
            var channelsResponse = channelsData.Select(c => new ChannelDto
            {
                ChannelId = c.ChannelId,
                Name = c.Name,
                Description = c.Description,
                LogoUrl = c.LogoUrl,
                ShowEvents = c.ShowEvents
                    .OrderBy(se => se.TimeStart)
                    .Select(se => new ShowEventDto
                {
                    ShowEventId = se.Id,
                    Description = se.Description,
                    TimeStart = se.TimeStart,
                    Duration = se.Duration,
                    ShowId = se.ShowId,
                })
                
            });
            
            
            // include all the shows
            var shows = await _DbContext.Shows
                .Include(s => s.Tag)
                .ToListAsync();

            var showsResponse = shows.Select(s => new ShowDto
            {
                ShowId = s.ShowId,
                Name = s.Name,
                TagName = s.Tag?.Name,
                ImageUrl = s.ImageUrl,
                ResizedImageUrl = s.ResizedImageUrl
            });
            
            //get recommendations
            var individualRecommendation = await _DbContext.IndividualRecommendations
                .Where(x => x.UserId == user.Id && x.CreatedDate == DateTime.Today)
                .Include(x => x.Show)
                .FirstOrDefaultAsync();

            var globalRecommendation = await _DbContext.GlobalRecommendations
                .Where(x => x.Active)
                .Include(x => x.Show)
                .FirstOrDefaultAsync();
            
            
            return Ok(new
            {
                schedule = response,
                favTags = favouriteTags,
                channels = channelsResponse,
                shows = showsResponse,
                globalRecommendation = globalRecommendation?.Show.ShowId,
                individualRecommendation = individualRecommendation?.Show.ShowId, // => x != null ? x : null
            });
        }

        // IF USER NOT AUTHENTICATED
        return Unauthorized();
    }



    [HttpPost("add-show-manually")]
    public async Task<IActionResult> AddShow([FromBody] AddShowManualDTO request)
    {
        // check if the channel exsists / get the channel
        var channel = await _DbContext.Channels.FirstOrDefaultAsync(chnl => chnl.ChannelId == request.ChannelId);
        var tag = await _DbContext.Tags.FirstOrDefaultAsync(tag => tag.Name == request.TagName)
            ?? _DbContext.Tags.Add(new Tag
            {
                Name = request.TagName
            }).Entity;
        
        if (channel == null)
            return NotFound("nah mate");

        // check if the show exsists > create or get
        var show = await _DbContext.Shows.FirstOrDefaultAsync(show => show.Name == request.ShowName)
            ?? _DbContext.Shows.Add(new Show
            {
                Name = request.ShowName,
                ImageUrl = request.ImageUrl,
                Tag = tag
            }).Entity;
        
        // create ShowEvent and append to the channel
        // LATER - check if theres an event with the same start time in the channel -> retrive channel with showevents?
        var showEvent = _DbContext.ShowEvents.Add(new ShowEvent
        {
            Channel = channel,
            Show = show,
            TimeStart = request.StartTime,
            Duration = request.Duration,
        });
        await _DbContext.SaveChangesAsync();
        return Ok(showEvent.Entity);
    }
    

    [HttpGet("testTagGetter")]
    public async Task<IActionResult> TestTagGetter([FromQuery] string showName)
    {
        var openAiHandler = new OpenAiHandler(_httpClient, new List<string> { "comedy", "drama", "news" }, showName);
        var result = await openAiHandler.RequestTag();
        
        return Ok(result);
    }


    [HttpGet("checkShowsCache")]
    public IActionResult CheckShowsCache()
    {
        var shows = _todaysShowsCache.GetCachedShows();

        if (shows == null)
        {
            return Ok("No cached shows");
        }
        return Ok(shows);
    }

    [HttpGet("cachedShowsTagHashmap")]
    public IActionResult CachedShowsTagHashmap()
    {
        var hashmap = _todaysShowsCache.GetShowTagHashmap();
        
        return Ok(hashmap);
    }

}