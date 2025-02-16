using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;

namespace tvscheduler.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly AppDbContext _DbContext;
    private readonly string _message = "u alright mate";
    private readonly UserManager<User> _userManager;
    
    

    public AccountController(AppDbContext dbContext, UserManager<User> userManager)
    {
        _DbContext = dbContext;
        _userManager = userManager;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Registration(LoginDTO user)
    {
        var exsistingUser = await _userManager.FindByNameAsync(user.Name);
        if (exsistingUser != null)
        {
            return BadRequest(new { message = "Username already exists!" });
        }

        var newUser = new User
        {
            UserName = user.Name
        };

        var result = await _userManager.CreateAsync(newUser, user.Password);

        if (result.Succeeded)
        {
            return Ok(new { message = "Registration success!" });
        }
        else
        {
            // Extract error messages from result.Errors
            var errors = result.Errors.Select(e => e.Description);

            return BadRequest(new 
            { 
                message = "Registration failed.", 
                errors
            });
        }
    }

    

    [HttpPost("add-show-to-schedule")]
    public async Task<IActionResult> AddShowToSchedule([FromBody] UserScheduleDTO request)
    {
        
        var showEvent = await _DbContext.ShowEvents.FirstOrDefaultAsync(ev => ev.Id == request.ShowEventId);
        if (showEvent == null)
        {
            return NotFound("Show event not found.");
        }

        var scheduleEvent = _DbContext.ScheduleEvents.Add(new UserScheduleEvent
            {
                ShowEventId = request.ShowEventId,
                UserId = request.UserId,
            });
        var result = await _DbContext.SaveChangesAsync();
         
        return Ok();
        
    }


    [HttpPost("remove-show-from-schedule")]
    public async Task<IActionResult> RemoveShowFromSchedule([FromBody] UserScheduleDTO request)
    {

        var showEvent = await _DbContext.ShowEvents.FirstOrDefaultAsync(ev => ev.Id == request.ShowEventId);
        if (showEvent == null)
        {
            return NotFound("Show event not found.");
        }

        var scheduleEvent = await _DbContext.ScheduleEvents.FirstOrDefaultAsync(se => 
            se.UserId == request.UserId && se.ShowEventId == request.ShowEventId);

        if (scheduleEvent == null)
        {
            return NotFound("Schedule event not found.");
        }

        _DbContext.ScheduleEvents.Remove(scheduleEvent);
        await _DbContext.SaveChangesAsync();

        return Ok("Show event removed from schedule.");
        
    }

}