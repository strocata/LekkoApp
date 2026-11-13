namespace LekkoApp.Core.Services.Models;

public record DashboardTotals(
    int TotalProjects,
    int TotalTasks,
    int CompletedTasks,
    int InProgressTasks,
    int OverdueTasks,
    int PomodorosToday,
    int PomodorosThisWeek,
    double AvgPomodorosPerDay);