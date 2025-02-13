using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using tvscheduler.network_services;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;

namespace tvscheduler.Controllers;


[Microsoft.AspNetCore.Components.Route("")]
[ApiController]
public class TvController : ControllerBase
{

    private readonly AppDbContext _DbContext;
    private readonly HttpClient _httpClient;

    public TvController(HttpClient httpClient, AppDbContext dbContext)
    {
        _httpClient = httpClient;
        _DbContext = dbContext;
    }





    [HttpGet("")]
    public async Task<IActionResult> Get()
    {
        var channelIds = new List<int> { 560, 700, 10005, 1540, 1547 };
        var guideData = await TvApi.FetchGuideData(_httpClient);

        if (guideData == null)
        {
            return StatusCode(502, new { error = "Failed to fetch TV guide data" });
        }

        var programData = await TvApi.FetchMultipleProgramData(_httpClient, channelIds);
        return Ok(new { guideData, programData });
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
                Description = request.Description,
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

}