using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

// Represents a global recommendation for all users
public class RecommendationGlobal
{
    [Key]
    public int Id { get; set; }
    
    public int ShowId { get; set; }
    [ForeignKey("ShowId")]
    public Show Show { get; set; }
    
    public DateTime Added { get; set; }
    public Boolean Active { get; set; }
}