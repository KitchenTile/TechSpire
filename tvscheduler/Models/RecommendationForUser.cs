using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

public class RecommendationForUser
{
    
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("User")]
    public string UserId { get; set; }
    public User User { get; set; }
    
    [ForeignKey("Show")]
    public int ShowId { get; set; }
    public Show Show { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
}