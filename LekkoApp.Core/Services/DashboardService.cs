using LekkoApp.Core.Services.Models;
using LekkoApp.Data;
using LekkoApp.Models;
using Microsoft.EntityFrameworkCore;
using TaskStatus = LekkoApp.Models.Enums.TaskStatus;


namespace LekkoApp.Core.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardData> GetForUserAsync(string userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var weekStart = today.AddDays(-6);

        var userTasks = _context.PomodoroTasks
            .AsNoTracking()
            .Where(t => t.UserId == userId && !t.IsDeleted);

        var userPomodoros = _context.Pomodoros
            .AsNoTracking()
            .Where(p => p.Task!.UserId == userId);

        var totals = await ComputeTotalsAsync(userTasks, userId, now, weekStart, ct);
        var tasksByStatus = await ComputeTasksByStatusAsync(userTasks, ct);
        var pomodorosByDay = await ComputePomodorosByDayAsync(userPomodoros, weekStart, ct);
        var overdueTasks = await LoadTaskListAsync(
            userTasks.Where(t => t.DueDate < now && t.Status != TaskStatus.Completed)
                .OrderBy(t => t.DueDate),
            limit: 5, ct);
        var activeTasks = await LoadTaskListAsync(
            userTasks.Where(t => t.Status == TaskStatus.InProgress)
                .OrderByDescending(t => t.UpdatedAt),
            limit: 5, ct);

        return new DashboardData(
            Totals: totals,
            TasksByStatus: tasksByStatus,
            PomodorosByDay: pomodorosByDay,
            OverdueTasks: overdueTasks,
            ActiveTasks: activeTasks);
    }
    
    private async Task<DashboardTotals> ComputeTotalsAsync(
        IQueryable<PomodoroTask> userTasks,
        string userId,
        DateTime now,
        DateTime weekStart,
        CancellationToken ct)
    {
        var total = await userTasks.CountAsync(ct);
        var completed = await userTasks.CountAsync(t => t.Status == TaskStatus.Completed, ct);
        var inProgress = await userTasks.CountAsync(t => t.Status == TaskStatus.InProgress, ct);
        var overdue = await userTasks.CountAsync(t => t.DueDate < now && t.Status != TaskStatus.Completed, ct);

        var projectsCount = await _context.Projects
            .CountAsync(p => p.User!.Id == userId, ct);

        var userPomodoros = _context.Pomodoros.Where(p => p.Task!.UserId == userId);
        var today = now.Date;
        var pomodorosToday = await userPomodoros.CountAsync(p => p.StartedAt >= today && p.StartedAt < today.AddDays(1), ct);
        var pomodorosWeek = await userPomodoros.CountAsync(p => p.StartedAt >= weekStart, ct);

        return new DashboardTotals(
            TotalProjects: projectsCount,
            TotalTasks: total,
            CompletedTasks: completed,
            InProgressTasks: inProgress,
            OverdueTasks: overdue,
            PomodorosToday: pomodorosToday,
            PomodorosThisWeek: pomodorosWeek,
            AvgPomodorosPerDay: pomodorosWeek / 7.0);
    }
    
    private static async Task<List<DailyCount>> ComputePomodorosByDayAsync(
        IQueryable<PomodoroSession> userPomodoros,
        DateTime weekStart,
        CancellationToken ct)
    {
        var raw = await userPomodoros
            .Where(p => p.StartedAt >= weekStart)
            .GroupBy(p => p.StartedAt.Date)
            .Select(g => new DailyCount(g.Key, g.Count()))
            .ToListAsync(ct);

        return Enumerable.Range(0, 7)
            .Select(i => weekStart.AddDays(i))
            .Select(date => raw.FirstOrDefault(r => r.Date == date) ?? new DailyCount(date, 0))
            .ToList();
    }
    
    private static async Task<List<StatusCount>> ComputeTasksByStatusAsync(
        IQueryable<PomodoroTask> userTasks,
        CancellationToken ct)
    {
        var raw = await userTasks
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return raw.Select(x => new StatusCount(x.Status, x.Count)).ToList();
    }
    
    private static async Task<List<TaskSummary>> LoadTaskListAsync(
        IQueryable<PomodoroTask> query,
        int limit,
        CancellationToken ct)
    {
        return await query
            .Take(limit)
            .Select(t => new TaskSummary(
                t.Id, t.Title, t.Description,
                t.Status, t.Priority, t.DueDate, t.ProjectId))
            .ToListAsync(ct);
    }
}