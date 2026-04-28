using System.ComponentModel.DataAnnotations;
using LekkoApp.Data;

namespace LekkoApp.Models;

using Enums;

public class PomodoroTask
{
    public Guid Id { get; set; }

    public required ApplicationUser? User { get; set; }

    public string UserId { get; set; } = string.Empty;

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public int TaskNumber { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [MaxLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
    public required string Title { get; set; }

    [MaxLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
    public string? Description { get; set; }

    [Range(1, 20, ErrorMessage = "Estimated Pomodoros must be between 1 and 20")]
    public int EstimatedPomodoros { get; set; } = 1;

    [Range(0, 100, ErrorMessage = "Completed Pomodoros cannot exceed 100")]
    public int CompletedPomodoros { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public ICollection<PomodoroSession>? PomodoroSessions { get; set; }

    public ICollection<Tag>? Tags { get; set; }

    public Priority Priority { get; set; } = Priority.Medium;

    public RecurrencePattern Recurrence { get; set; } = RecurrencePattern.None;

    public Guid? ParentTaskId { get; set; }
    public PomodoroTask? ParentTask { get; set; }
    public ICollection<PomodoroTask>? Subtasks { get; set; }

    public bool IsDeleted { get; set; }
}