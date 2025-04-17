namespace tvscheduler.Models;


// Data transfer object for the main endpoint response containing user's TV guide data
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
    public required string? ResizedImageUrl { get; set; }
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