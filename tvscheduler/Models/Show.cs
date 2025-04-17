using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tvscheduler.Models;

// Represents a TV show with its metadata and associated tag
public class Show
{
    [Key]
    public int ShowId { get; set; } 

    [MaxLength(100)]
    public required string Name { get; set; }
    [MaxLength(100)]
    public required string ImageUrl { get; set; }
    [MaxLength(100)]
    public string? ResizedImageUrl { get; set; }
    public Tag? Tag { get; set; }
    
}