using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using tvscheduler.network_services;
using System.Net.Http;

namespace tvscheduler.Controllers;


[Microsoft.AspNetCore.Components.Route("")]
[ApiController]
public class TvController : ControllerBase
{

    private readonly HttpClient _httpClient;

    public TvController(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
}