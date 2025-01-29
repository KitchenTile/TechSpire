namespace tvscheduler;

public class Channel
{
    public required int ChannelId { get; set; }
    public required string Name { get; set; }
    public required List<Show> ShowList { get; set; }

    // constructor
    public Channel(int channelId, string name, List<Show> showlist)
    {
        channelId = CannelId;
        name = Name;
        showlist = ShowList;
    }

    public void DisplayChannel()
    {
        Console.WriteLine("Channel Name: " + channelName);
        Console.WriteLine("Channel Description: " + channelDescription);
        Console.WriteLine("Program List: " + progrmaList);
    }

    public void AddProgram(Program program)
    {
        progrmaList += program.programName + ", ";
    }
}