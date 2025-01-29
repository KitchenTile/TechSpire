namespace tvscheduler;

public class Show
 {
     public required int Id { get; set; }
     public required string Name { get; set; }
     public required int StartTime { get; set; }
     public required int EndTime { get; set; }
     public required int ChannelId { get; set; }

    // constructor
    public Show(int id, string name, int startTime, int endTime, int channelId)
    {
        id = Id;
        name = Name;
        startTime = StartTime;
        endTime = EndTime;
        channelId = ChannelId;
    }
}