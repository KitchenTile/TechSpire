using System.ComponentModel.DataAnnotations;
using tvscheduler.Models;

namespace tvscheduler;

// Represents a TV channel with its schedule and associated tags
public class Channel
{
    [Key]
    public required int ChannelId { get; set; }
    
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int Lcn { get; set; }
    public required string LogoUrl { get; set; }
    public required bool Tstv { get; set; }
    
    public List<ShowEvent> ShowEvents { get; set; } = new List<ShowEvent>();
    
    public List<ChannelTag> ChannelTags { get; set; } = new();

}