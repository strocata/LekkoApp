using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LekkoApp.Models;

public class TimerLog
{
    [Key]
    public Guid Id { get; set; }

    public required string TimerType { get; set; }
    public DateTime CompletedAt { get; set; }

    public Guid TaskId { get; set; }
    public Task Task { get; set; }
}

