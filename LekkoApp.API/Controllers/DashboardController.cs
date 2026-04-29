using LekkoApp.API.DTOs.Responses;
using LekkoApp.Data;
using LekkoApp.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.API.Controllers;

public class DashboardController : BaseApiController
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns aggregated dashboard statistics for the current user.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Get()
    {
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-6);

        var tasks = _context.PomodoroTasks.Where(t => !t.IsDeleted);

        var dailyCounts = await _context.Pomodoros
            .Where(p => p.StartedAt >= weekStart)
            .GroupBy(p => p.StartedAt.Date)
            .Select(g => g.Count())
            .ToListAsync();

        var tasksByStatus = (await tasks
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync())
            .Select(x => new StatItemResponse(x.Status.ToString(), x.Count))
            .ToList();

        var pomodorosByDay = (await _context.Pomodoros
            .Where(p => p.StartedAt.Date >= weekStart)
            .GroupBy(p => p.StartedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync())
            .Select(x => new DailyStatResponse(x.Date, x.Count))
            .ToList();

        var overdueTaskList = (await tasks
            .Where(t => t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed)
            .OrderBy(t => t.DueDate)
            .Take(5)
            .Select(t => new { t.Id, t.Title, t.Description, t.Status, t.Priority, t.DueDate, t.ProjectId })
            .ToListAsync())
            .Select(t => new TaskSummaryResponse(
                t.Id, t.Title, t.Description,
                t.Status.ToString(), t.Priority.ToString(),
                t.DueDate, t.ProjectId))
            .ToList();

        var activeTasks = (await tasks
            .Where(t => t.Status == TaskStatus.InProgress)
            .Take(5)
            .Select(t => new { t.Id, t.Title, t.Description, t.Status, t.Priority, t.DueDate, t.ProjectId })
            .ToListAsync())
            .Select(t => new TaskSummaryResponse(
                t.Id, t.Title, t.Description,
                t.Status.ToString(), t.Priority.ToString(),
                t.DueDate, t.ProjectId))
            .ToList();

        var response = new DashboardResponse(
            TotalProjects: await _context.Projects.CountAsync(),
            TotalTasks: await tasks.CountAsync(),
            CompletedTasks: await tasks.CountAsync(t => t.Status == TaskStatus.Completed),
            InProgressTasks: await tasks.CountAsync(t => t.Status == TaskStatus.InProgress),
            OverdueTasks: await tasks.CountAsync(t =>
                t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed),
            PomodorosToday: await _context.Pomodoros.CountAsync(p => p.StartedAt.Date == today),
            PomodorosThisWeek: await _context.Pomodoros.CountAsync(p => p.StartedAt.Date >= weekStart),
            AvgPomodorosPerDay: dailyCounts.Count > 0 ? dailyCounts.Average() : 0,
            TasksByStatus: tasksByStatus,
            PomodorosByDay: pomodorosByDay,
            OverdueTasks5: overdueTaskList,
            ActiveTasks5: activeTasks
        );

        return Ok(response);
    }
}