using Microsoft.EntityFrameworkCore;
using tvscheduler.Models;

namespace tvscheduler;

public class AppDbContext : DbContext
{
    public  DbSet<Show> Shows { get; set; }
    public DbSet<Channel> Channels { get; set; }
    
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
}