namespace LekkoApp.Core.Services.Models;

public record DashboardData(
    DashboardTotals Totals,
    IReadOnlyList<StatusCount> TasksByStatus,
    IReadOnlyList<DailyCount> PomodorosByDay,
    IReadOnlyList<TaskSummary> OverdueTasks,
    IReadOnlyList<TaskSummary> ActiveTasks);