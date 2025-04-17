namespace tvscheduler.Models;

// Request object for setting multiple favorite tags at once
public class SetFavTagsRequest
{
    public required List<int> tagIds { get; set; }
}