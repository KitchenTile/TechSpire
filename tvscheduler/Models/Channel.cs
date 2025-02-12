namespace tvscheduler;

public class Channel
{
    public required int ChannelId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int Lcn { get; set; }
    public required string LogoUrl { get; set; }
    public required bool Tstv { get; set; }
}