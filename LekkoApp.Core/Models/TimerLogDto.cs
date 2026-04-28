namespace LekkoApp.Models;

public class TimerSessionStartDto
{
    public Guid TaskId { get; set; }
    public PomodoroType? PomodoroType { get; set; } = null;
}

public class TimerSessionFinishDto
{
    public Guid SessionId { get; set; }
}

public class TimerSessionInterruptDto
{
    public Guid SessionId { get; set; }
    public string Reason { get; set; } = null!;
}

public class TimerLogDto
{
    public Guid TaskId { get; set; }
    public string TimerType { get; set; } = null!;
}

