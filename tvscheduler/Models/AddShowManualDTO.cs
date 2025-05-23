using System.ComponentModel.DataAnnotations;

namespace tvscheduler.Models;

// Data transfer object for manually adding a new show to the TV guide
public class AddShowManualDTO
{
    
    public required int ChannelId { get; set; }
    public required string ShowName { get; set; }
    public required string Description { get; set; }
    public required int StartTime { get; set; }
    public required int Duration { get; set; }
    public required string TagName { get; set; }
    public required string ImageUrl { get; set; }
    
}