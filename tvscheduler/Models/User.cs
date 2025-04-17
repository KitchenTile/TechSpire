using Microsoft.AspNetCore.Identity;

namespace tvscheduler.Models;

// Represents a user in the TV scheduler system, extending IdentityUser for authentication
public class User : IdentityUser
{
    public List<ShowEvent> MySchedule { get; set; } = new List<ShowEvent>();
    public List<FavouriteTag> FavouriteTags { get; set; }
    public List<FavouriteShow> FavouriteShows { get; set; }
}