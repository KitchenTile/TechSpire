using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using tvscheduler.Models;

namespace tvscheduler.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly AppDbContext _DbContext;
    private readonly string _message = "u alright mate";
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    
    

    public AccountController(AppDbContext dbContext, UserManager<User> userManager, IConfiguration configuration)
    {
        _DbContext = dbContext;
        _userManager = userManager;
        _configuration = configuration;
        
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
    
    // login
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginDTO request)
    {
        var user = await _userManager.FindByNameAsync(request.Name);
        if (user != null)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signIn);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        return Unauthorized(new { message = "Invalid email or password" }); 
    }
    
    
    

    [HttpPost("add-show-to-schedule")]
    public async Task<IActionResult> AddShowToSchedule([FromBody] AddShowToScheduleRequest request)
    {
        /// HTTP CONTEXT
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var showEvent = await _DbContext.ShowEvents.FirstOrDefaultAsync(ev => ev.Id == request.ShowEventId);
        if (showEvent == null)
        {
            return NotFound("Show event not found.");
        }

        var scheduleEvent = _DbContext.ScheduleEvents.Add(new UserScheduleEvent
            {
                ShowEventId = request.ShowEventId,
                UserId = userId,
            });
        var result = await _DbContext.SaveChangesAsync();
         
        return Ok();
        
    }


    [HttpPost("remove-show-from-schedule")]
    public async Task<IActionResult> RemoveShowFromSchedule([FromBody] AddShowToScheduleRequest request)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var showEvent = await _DbContext.ShowEvents.FirstOrDefaultAsync(ev => ev.Id == request.ShowEventId);
        if (showEvent == null)
        {
            return NotFound("Show event not found.");
        }

        var scheduleEvent = await _DbContext.ScheduleEvents.FirstOrDefaultAsync(se => 
            se.UserId == userId && se.ShowEventId == request.ShowEventId);

        if (scheduleEvent == null)
        {
            return NotFound("Schedule event not found.");
        }

        _DbContext.ScheduleEvents.Remove(scheduleEvent);
        await _DbContext.SaveChangesAsync();

        return Ok("Show event removed from schedule.");
    }
}