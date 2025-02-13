using Microsoft.AspNetCore.Identity;

namespace tvscheduler.Models;

public class User : IdentityUser
{
    public List<ShowEvent>? ShowEvents { get; set; }
    public List<Tag>? FavouriteTags { get; set; }
}