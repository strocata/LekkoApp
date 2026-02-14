using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.Models;

public class ProjectDetailsViewModel
{
    public Project? Project { get; set; }
    public List<PomodoroTask>? PomodoroTasks { get; set; }
    public int EstimatedPomodoroCount { get; set; }
    public int DonePomodoroCount { get; set; }
    public int PomodoroTasksCount => PomodoroTasks?.Count ?? 0;
    public int DoneTasksCount =>
        PomodoroTasks.Count(item => item.Status == TaskStatus.Completed);
}