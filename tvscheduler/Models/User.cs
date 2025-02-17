using Microsoft.AspNetCore.Identity;

namespace tvscheduler.Models;

public class User : IdentityUser
{
    public List<ShowEvent> MySchedule { get; set; } = new List<ShowEvent>();
    public List<Tag>? FavouriteTags { get; set; }
}