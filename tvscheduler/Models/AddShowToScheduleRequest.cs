using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

// Request object for adding a show to user's schedule
public class AddShowToScheduleRequest
{
    public int ShowEventId { get; set; }
    
}