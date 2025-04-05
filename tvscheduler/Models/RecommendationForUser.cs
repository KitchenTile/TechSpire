using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

public class RecommendationForUser
{
    
    [Key]
    public int Id { get; set; }
    
    
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    
    public int ShowId { get; set; }
    [ForeignKey("ShowId")]
    public Show Show { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
}