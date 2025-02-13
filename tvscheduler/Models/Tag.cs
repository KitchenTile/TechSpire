using System.ComponentModel.DataAnnotations;

namespace tvscheduler.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<ChannelTag> ChannelTags { get; set; } = new();

}