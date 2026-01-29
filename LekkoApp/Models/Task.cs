using System.ComponentModel.DataAnnotations;
using LekkoApp.Data;

namespace LekkoApp.Models;
using Enums;

public class Task
{
    public Guid Id { get; set; }
    public required ApplicationUser? User { get; set; }
    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }
    public int TaskNumber { get; set; }
    [MaxLength(50)]
    public required string Title { get; set; }
    [MaxLength(300)]
    public string? Description { get; set; }

    public int EstimatedPomodoros { get; set; } = 1;
    public int CompletedPomodoros { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }

    public ICollection<PomodoroSession>? PomodoroSessions { get; set; }

    public ICollection<Tag>? Tags { get; set; }

    public Priority Priority { get; set; } = Priority.Medium;
    public RecurrencePattern Recurrence { get; set; } = RecurrencePattern.None;

    public Guid? ParentTaskId { get; set; }
    public Task? ParentTask { get; set; }
    public ICollection<Task>? Subtasks { get; set; }
}
