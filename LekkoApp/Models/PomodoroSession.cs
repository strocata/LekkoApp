namespace LekkoApp.Models;

public class PomodoroSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public Guid TaskId { get; set; }
    public Task? Task { get; set; } = null;

    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public PomodoroType? Type { get; set; }

    public PomodoroSessionStatus Status { get; set; }

    public string? InterruptReason { get; set; }
}

public enum PomodoroType
{
    Work,
    ShortBreak,
    LongBreak
}

public enum PomodoroSessionStatus
{
    Active, // стартувала, таймер іде
    Completed, // завершилась по таймеру
    Interrupted // завершена вручну
}