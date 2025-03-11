using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

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