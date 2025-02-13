using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

public class Show
{
    [Key]
    public int ShowId { get; set; } 

    [MaxLength(100)]
    public required string Name { get; set; }
    [MaxLength(100)]
    public string? Description { get; set; }
    [MaxLength(100)]
    public required string ImageUrl { get; set; }
    public Tag? Tag { get; set; }
    
}