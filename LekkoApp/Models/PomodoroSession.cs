namespace LekkoApp.Models;

public class PomodoroSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public Task Task { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public PomodoroType Type { get; set; } // Work / ShortBreak / LongBreak
    public bool IsCompleted { get; set; }
}

public enum PomodoroType
{
    Work,
    ShortBreak,
    LongBreak
}
