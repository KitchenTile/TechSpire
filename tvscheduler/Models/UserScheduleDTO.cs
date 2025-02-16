using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

public class UserScheduleDTO
{
    public string UserId { get; set; }
    public int ShowEventId { get; set; }
    
}