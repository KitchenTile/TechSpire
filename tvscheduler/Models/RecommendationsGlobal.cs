using System.ComponentModel.DataAnnotations;

namespace tvscheduler.Models;

public class RecommendationsGlobal
{
    [Key]
    public int Id { get; set; }
    
    public int ShowId { get; set; }
    public Show Show { get; set; }
    
    public DateTime Added { get; set; }
    public Boolean Active { get; set; }
}