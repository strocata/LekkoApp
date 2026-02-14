using LekkoApp.Helpers;

namespace LekkoApp.Models;

public class PomodoroTasksViewModel
{
    public PaginatedList<PomodoroTask>? PomodoroTasks { get; set; }
    public PomodoroTask? SelectedPomodoroTask { get; set; }
}