namespace LekkoApp.API.DTOs.Responses;

public record DashboardResponse(
    int TotalProjects,
    int TotalTasks,
    int CompletedTasks,
    int InProgressTasks,
    int OverdueTasks,
    int PomodorosToday,
    int PomodorosThisWeek,
    double AvgPomodorosPerDay,
    List<StatItemResponse> TasksByStatus,
    List<DailyStatResponse> PomodorosByDay,
    List<TaskSummaryResponse> OverdueTasks5,
    List<TaskSummaryResponse> ActiveTasks5
);

public record StatItemResponse(string Label, int Value);

public record DailyStatResponse(DateTime Date, int Value);

public record TaskSummaryResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTime? DueDate,
    Guid? ProjectId
);