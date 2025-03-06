namespace tvscheduler.Models;


//rename to main endpoint response and create a method which will construct the reponse to the main endpoint fully
public class MainEndpointResponseDto
{
    public int UserScheduleItemId { get; set; }
    public required ShowEventDto ShowEvent { get; set; }
}

public class ShowDto
{
    public int ShowId { get; set; }
    public required string Name { get; set; }
    public string? TagName { get; set; }
    public required string ImageUrl { get; set; }
}

public class ShowEventDto
{
    public int ShowEventId { get; set; }
    public int ChannelId { get; set; }
    //public ShowDto? Show { get; set; }
    
    public string? Description { get; set; }
    public int? ShowId { get; set; }
    public int TimeStart { get; set; }
    public int Duration { get; set; }
}


public class ChannelDto
{
    public int ChannelId { get; set; }
    public string Name { get; set; }
    public string LogoUrl { get; set; }
    public string Description { get; set; }
    public IEnumerable<ShowEventDto> ShowEvents { get; set; }
}