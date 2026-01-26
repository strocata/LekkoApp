using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.Models;

public class ProjectDetailsViewModel
{
    public Project? Project { get; set; }
    public List<Task>? Tasks { get; set; }
    public int EstimatedPomodoroCount { get; set; }
    public int DonePomodoroCount { get; set; }
    public int TasksCount => Tasks?.Count ?? 0;
    public int DoneTasksCount =>
        Tasks.Count(item => item.Status == TaskStatus.Completed);
}