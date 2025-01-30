namespace tvscheduler;

public class Show
{
    public required int evtID { get; set; }
    public required string Name { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required int Channel { get; set; }
}