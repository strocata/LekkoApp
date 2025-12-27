using LekkoApp.Data;
using Microsoft.AspNetCore.Mvc;
using LekkoApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Task = LekkoApp.Models.Task;
using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.Controllers;

[ApiController]
[Route("[controller]")]
public class TimerLogController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TimerLogController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [Authorize]
    [HttpPost("LogIteration")]
    public async Task<IActionResult> LogIteration([FromBody] TimerLogDto dto)
    {
        var currentTask = await _context.Tasks.FindAsync(dto.TaskId);
        if (currentTask == null)
        {
            return NotFound(new { success = false, message = "Task not found" });
        }

        if (dto.TimerType == "pomodoro-timer")
        {
            currentTask.CompletedPomodoros++;
        }

        if (currentTask.Status == TaskStatus.Completed)
        {
            return BadRequest(new { success = false, message = "Task already completed" });
        }
        
        await _context.SaveChangesAsync();

        int finishedSessions = currentTask.CompletedPomodoros;

        int plannedSessions = currentTask.EstimatedPomodoros;

        bool isCompleted = false;
        if (finishedSessions >= plannedSessions)
        {
            currentTask.Status = TaskStatus.Completed;
            isCompleted = true;
            await _context.SaveChangesAsync();
        }

        return Ok(new 
        {
            success = true,
            completed = isCompleted,
            status = currentTask.Status.ToString(),
            completedPomodoros = currentTask.CompletedPomodoros.ToString(),
            estimatedPomodoros = currentTask.EstimatedPomodoros.ToString(),
        });
    }


}