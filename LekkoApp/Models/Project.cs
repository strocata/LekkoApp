using System.ComponentModel.DataAnnotations;
using LekkoApp.Data;
using LekkoApp.Models.Enums;

namespace LekkoApp.Models;

public class Project
{
    public Guid Id { get; set; }
    [MaxLength(50)]
    public required string Name { get; set; }
    [MaxLength(300)]
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public ProjectStatus Status { get; set; }
    public required ApplicationUser? User { get; set; }
    public DateTime CreatedAt { get; set; }
}

