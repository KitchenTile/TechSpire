namespace tvscheduler.Models;

// Represents the association between a channel and its tags
public class ChannelTag
{
    public int Id { get; set; }
    
    public int ChannelId { get; set; }
    public Channel Channel { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}