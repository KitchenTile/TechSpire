using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

public class AddToScheduleDTO
{
    public string UserId { get; set; }
    public int ShowEventId { get; set; }
    
}