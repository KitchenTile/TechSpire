using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;

namespace tvscheduler;

public class AppDbContext : IdentityDbContext<User>
{
    public  DbSet<Show> Shows { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<ShowEvent> ShowEvents { get; set; }
    public DbSet<Tag> Tags { get; set; }
    
    public DbSet<ChannelTag> ChannelTags { get; set; }
    public DbSet<UserScheduleEvent> ScheduleEvents { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
}