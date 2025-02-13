using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

public class ShowEvent
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("Channel")]
    public int ChannelId { get; set; }
    public required Channel Channel { get; set; }

    [ForeignKey("Show")]
    public int ShowId { get; set; }
    public required Show Show { get; set; }

    public int TimeStart { get; set; }
    public int Duration { get; set; }
}