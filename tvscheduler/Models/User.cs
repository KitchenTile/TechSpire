using Microsoft.AspNetCore.Identity;

namespace tvscheduler.Models;

public class User : IdentityUser
{
    public List<ShowEvent> MySchedule { get; set; } = new List<ShowEvent>();
    public List<FavouriteTag> FavouriteTags { get; set; }
    public List<FavouriteShow> FavouriteShows { get; set; }
}