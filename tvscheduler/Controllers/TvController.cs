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

namespace tvscheduler.Controllers;


[Microsoft.AspNetCore.Components.Route("")]
[ApiController]
public class TvController : ControllerBase
{

    private readonly AppDbContext _DbContext;
    private readonly HttpClient _httpClient;
    private readonly UserManager<User> _userManager;

    public TvController(HttpClient httpClient, AppDbContext dbContext, UserManager<User> userManager)
    {
        _httpClient = httpClient;
        _DbContext = dbContext;
        _userManager = userManager;
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
            });
            
            return Ok(new { schedule = response, channels = channelsResponse, shows = showsResponse });
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
    
    
    // 
    
    [HttpGet("old-endpoint")]
    public async Task<IActionResult> Get()
    {
        var channelIds = new List<int> { 10005, 1540, 1547 };
        var guideData = await TvApi.FetchGuideData(_httpClient);

        if (guideData == null)
        {
            return StatusCode(502, new { error = "Failed to fetch TV guide data" });
        }

        var programData = await TvApi.FetchMultipleProgramData(_httpClient, channelIds);
        return Ok(new { guideData, programData });
    }



}