using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;

namespace tvscheduler;

// Database context for the TV scheduler application
public class AppDbContext : IdentityDbContext<User>
{
    // Core entities
    public DbSet<Show?> Shows { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<ShowEvent> ShowEvents { get; set; }
    public DbSet<Tag> Tags { get; set; }
    
    // User preferences and scheduling
    public DbSet<ChannelTag> ChannelTags { get; set; }
    public DbSet<UserScheduleEvent> ScheduleEvents { get; set; }
    public DbSet<FavouriteTag> FavouriteTags { get; set; }
    public DbSet<FavouriteShow> FavouriteShows { get; set; }
    
    // Recommendation system
    public DbSet<RecommendationGlobal> GlobalRecommendations { get; set; }
    public DbSet<RecommendationForUser> IndividualRecommendations { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
}