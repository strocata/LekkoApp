using LekkoApp.Data;
using LekkoApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TimerLogController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TimerLogController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Tasks/sessions are looked up only among the caller's own records;
    // anything else is reported as "not found" so foreign ids are not revealed.
    private string? CurrentUserId => _userManager.GetUserId(User);

    private Task<PomodoroTask?> FindOwnTaskAsync(Guid taskId) =>
        _context.PomodoroTasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == CurrentUserId);

    [HttpPost("start")]
    public async Task<IActionResult> StartSession([FromBody] TimerSessionStartDto dto)
    {
        var task = await FindOwnTaskAsync(dto.TaskId);
        if (task == null)
            return NotFound("PomodoroTask not found");

        if (task.Status == TaskStatus.Completed)
            return BadRequest("PomodoroTask already completed");

        var hasActiveSession = await _context.Pomodoros
            .AnyAsync(s => s.TaskId == dto.TaskId && s.EndedAt == null);

        if (hasActiveSession)
            return BadRequest("Active timer session already exists");

        var session = new PomodoroSession
        {
            Id = Guid.NewGuid(),
            TaskId = dto.TaskId,
            Type = dto.PomodoroType,
            StartedAt = DateTime.UtcNow
        };

        _context.Pomodoros.Add(session);
        await _context.SaveChangesAsync();

        return Ok(new { sessionId = session.Id });
    }

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteSession([FromBody] TimerSessionFinishDto dto)
    {
        var session = await _context.Pomodoros
            .Include(s => s.Task)
            .FirstOrDefaultAsync(s => s.Id == dto.SessionId && s.Task!.UserId == CurrentUserId);

        if (session == null)
            return NotFound("Session not found");

        if (session.EndedAt != null)
            return BadRequest("Session already finished");

        session.EndedAt = DateTime.UtcNow;
        session.Status = PomodoroSessionStatus.Completed;

        if (session.Type == PomodoroType.Work && session.Task != null)
        {
            session.Task.CompletedPomodoros++;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }


    [HttpPost("interrupt")]
    public async Task<IActionResult> InterruptSession([FromBody] TimerSessionInterruptDto dto)
    {
        var session = await _context.Pomodoros
            .FirstOrDefaultAsync(s => s.Id == dto.SessionId && s.Task!.UserId == CurrentUserId);
        if (session == null)
            return NotFound("Session not found");

        if (session.EndedAt != null)
            return BadRequest("Session already finished");

        session.EndedAt = DateTime.UtcNow;
        session.InterruptReason = dto.Reason;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("LogIteration")]
    public async Task<IActionResult> LogIteration([FromBody] TimerLogDto dto)
    {
        var task = await FindOwnTaskAsync(dto.TaskId);
        if (task == null)
            return NotFound("PomodoroTask not found");

        if (task.Status == TaskStatus.Completed)
        {
            return Ok(new
            {
                completed = true,
                completedPomodoros = task.CompletedPomodoros,
                estimatedPomodoros = task.EstimatedPomodoros
            });
        }

        if (dto.TimerType == "pomodoro-timer")
        {
            task.CompletedPomodoros++;
        }

        bool completed = false;

        if (task.CompletedPomodoros >= task.EstimatedPomodoros)
        {
            task.Status = TaskStatus.Completed;
            completed = true;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            completed,
            status = task.Status.ToString(),
            completedPomodoros = task.CompletedPomodoros,
            estimatedPomodoros = task.EstimatedPomodoros
        });
    }
}