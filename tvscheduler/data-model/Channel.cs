namespace tvscheduler;

public class Channel
{
    public required int ChannelId { get; set; }
    public required string Name { get; set; }
    public required string ChannelDescription { get; set; }
    public required List<Show> ShowList { get; set; }
    

    public void AddShow(Show show)
    {
        ShowList.Add(show);
    }
}