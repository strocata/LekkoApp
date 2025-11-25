namespace LekkoApp.Models;
using System.ComponentModel.DataAnnotations;

public class Task
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int TaskNumber { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int EstimatedPomodoros { get; set; }
    public int CompletedPomodoros { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    
    public ICollection<PomodoroSession>? PomodoroSessions { get; set; }
    public ICollection<TimerLog>? TimerLogs { get; set; }
}

public enum TaskStatus
{
    [Display(Name = "Not Started")]
    NotStarted = 0,
    [Display(Name = "In Progress")]
    InProgress = 1,
    [Display(Name = "Done")]
    Completed = 2,
    [Display(Name = "On Hold")]
    OnHold = 3,
    [Display(Name = "Cancelled")]
    Cancelled = 4
}
