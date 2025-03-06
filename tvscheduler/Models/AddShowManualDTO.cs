using System.ComponentModel.DataAnnotations;

namespace tvscheduler.Models;

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