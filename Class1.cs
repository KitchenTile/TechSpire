using System;

// channel class
public class Channel
{
    public string channelName { get; set; }
    public string channelDescription { get; set; }
    public string progrmaList { get; set; } // for now it is a string, but it should be a list of programs

    // constructor
    public Channel(string channelName, string channelDescription, string programList)
    {
        channelName = channelName;
        channelDescription = channelDescription;
        programList = programList;
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


//a