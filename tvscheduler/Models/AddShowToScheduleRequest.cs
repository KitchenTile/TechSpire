using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

public class AddShowToScheduleRequest
{
    public int ShowEventId { get; set; }
    
}