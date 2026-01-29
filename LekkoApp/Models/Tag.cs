using System.ComponentModel.DataAnnotations;
using LekkoApp.Data;

namespace LekkoApp.Models;

public class Tag
{
    public Guid Id { get; set; }

    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(20)]
    public string Color { get; set; } = "#206bc4"; // Default blue

    public string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public ICollection<Task>? Tasks { get; set; }
}
