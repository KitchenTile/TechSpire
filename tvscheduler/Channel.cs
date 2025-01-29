namespace tvscheduler;

public class Channel
{
    public required int ChannelId { get; set; }
    public required string Name { get; set; }
    public required string ChannelDescription  { get; set; }
    public required List<Show> ShowList { get; set; }

    // constructor
    // public Channel(int channelId, string name, List<Show> showlist)
    // {
    //     ChannelId = channelId;
    //     Name = name;
    //     ShowList = showlist;
    // }

    public void DisplayChannel()
    {
        Console.WriteLine("Channel Name: " + Name);
        Console.WriteLine("Channel Description: " + ChannelDescription);
        Console.WriteLine("Program List: " + ShowList);
    }

    public void AddShow(Show show)
    {
        ShowList.Add(show);
    }
}