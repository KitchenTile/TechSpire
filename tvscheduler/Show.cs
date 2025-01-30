namespace tvscheduler;

public class Show
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int StartTime { get; set; }
    public required int EndTime { get; set; }
    public required List<Channel> ChannelId{ get; set; }
}