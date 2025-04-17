using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

// Represents a show that a user has added to their personal schedule
public class UserScheduleEvent
{ 
    [Key] 
    public int Id { get; set; }

    [ForeignKey("User")]
    public required string UserId { get; set; }
    public User? User { get; set; }

    [ForeignKey("ShowEvent")]
    public required int ShowEventId { get; set; }
    public ShowEvent ShowEvent { get; set; } = null!;
}