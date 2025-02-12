using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

public class Show
{
    [Key]
    public int ShowId { get; set; } 

    [ForeignKey("Channel")]
    public int ChannelId { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public long StartTime { get; set; }
    public int Duration { get; set; }
    public required Channel Channel { get; set; }
}