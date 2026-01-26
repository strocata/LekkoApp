using LekkoApp.Data;
using LekkoApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LekkoApp.Models.Enums;
using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly ApplicationDbContext _context;

    public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-6);

        var tasks = _context.Tasks.AsQueryable();

        var dailyCounts = await _context.Pomodoros
            .Where(p => p.StartedAt >= weekStart)
            .GroupBy(p => p.StartedAt.Date)
            .Select(g => g.Count())
            .ToListAsync();

        var model = new DashboardViewModel
        {
            TotalProjects = await _context.Projects.CountAsync(),

            TotalTasks = await tasks.CountAsync(),
            CompletedTasks = await tasks.CountAsync(t => t.Status == TaskStatus.Completed),
            InProgressTasks = await tasks.CountAsync(t => t.Status == TaskStatus.InProgress),
            OverdueTasks = await tasks.CountAsync(t =>
                t.DueDate < DateTime.UtcNow &&
                t.Status != TaskStatus.Completed),

            PomodorosToday = await _context.Pomodoros
                .CountAsync(p => p.StartedAt.Date == today),

            PomodorosThisWeek = await _context.Pomodoros
                .CountAsync(p => p.StartedAt.Date >= weekStart),

            AvgPomodorosPerDay = dailyCounts.Any()
                ? dailyCounts.Average()
                : 0,

            TasksByStatus = await tasks
                .GroupBy(t => t.Status)
                .Select(g => new StatItem
                {
                    Label = g.Key.ToString(),
                    Value = g.Count()
                })
                .ToListAsync(),

            PomodorosByDay = await _context.Pomodoros
                .Where(p => p.StartedAt.Date >= weekStart)
                .GroupBy(p => p.StartedAt.Date)
                .Select(g => new DailyStat
                {
                    Date = g.Key,
                    Value = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync(),

            OverdueTaskList = await tasks
                .Where(t => t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed)
                .OrderBy(t => t.DueDate)
                .Take(5)
                .ToListAsync(),

            ActiveTasks = await tasks
                .Where(t => t.Status == TaskStatus.InProgress)
                // .OrderByDescending(t => t.UpdatedAt)
                .Take(5)
                .ToListAsync()
        };

        return View(model);
    }
}