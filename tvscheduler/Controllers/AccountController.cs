using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace tvscheduler.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly string _message = "u alright mate";



    [HttpGet]
    [Route("")]
    public IActionResult Test()
    {
        return Ok(this._message);
    }
}