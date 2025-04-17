using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

// Represents a scheduled broadcast of a show on a specific channel
public class ShowEvent
{
    [Key]
    public int Id { get; set; }
    
    public string Description { get; set; }
    
    [ForeignKey("Channel")]
    public int ChannelId { get; set; }
    public Channel? Channel { get; set; }

    [ForeignKey("Show")]
    public int ShowId { get; set; }
    public Show Show { get; set; }

    public int TimeStart { get; set; }
    public int Duration { get; set; }
}