namespace tvscheduler.Models;

// Data transfer object for user login credentials
public class LoginDTO
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}