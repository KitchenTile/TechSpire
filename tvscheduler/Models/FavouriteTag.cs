using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

// Represents a tag that a user has marked as their favorite
public class FavouriteTag
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("User")]
    public string UserId { get; set; }
    public User User { get; set; }
    
    [ForeignKey("Tag")]
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}