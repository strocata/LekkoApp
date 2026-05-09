using LekkoApp.API.DTOs.Requests;
using LekkoApp.API.DTOs.Responses;
using LekkoApp.Data;
using LekkoApp.Models;
using LekkoApp.Models.Enums;
using LekkoApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.API.Controllers;

public class TasksController : BaseApiController
{
    private readonly ITaskRepository _taskRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<TasksController> _logger;

    public TasksController(
        ITaskRepository taskRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<TasksController> logger)
    {
        _taskRepository = taskRepository;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> Get(
        string? search,
        string? sort,
        int page = 1,
        int pageSize = 10)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var query = _taskRepository.GetByUserQueryable(user).Where(t => !t.IsDeleted);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t =>
                t.Title.Contains(search) ||
                t.TaskNumber.ToString() == search ||
                (t.Description != null && t.Description.Contains(search)));
        }

        query = sort switch
        {
            "title" => query.OrderBy(t => t.Title),
            "title_desc" => query.OrderByDescending(t => t.Title),
            "due_date" => query.OrderBy(t => t.DueDate),
            "due_date_desc" => query.OrderByDescending(t => t.DueDate),
            "priority" => query.OrderBy(t => t.Priority),
            "priority_desc" => query.OrderByDescending(t => t.Priority),
            "created_at" => query.OrderBy(t => t.CreatedAt),
            "created_at_desc" => query.OrderByDescending(t => t.CreatedAt),
            "updated_at" => query.OrderBy(t => t.UpdatedAt),
            "updated_at_desc" => query.OrderByDescending(t => t.UpdatedAt),
            _ => query.OrderBy(t => t.TaskNumber)
        };

        var total = await query.CountAsync();
        var tasks = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        Response.Headers["X-Total-Count"] = total.ToString();

        return Ok(tasks.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null) return NotFound();
        if (task.UserId != user.Id) return Forbid();

        return Ok(MapToResponse(task));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create([FromBody] CreateTaskRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (request.DueDate.HasValue && request.DueDate.Value < DateTime.UtcNow)
            return BadRequest(new { error = "Due date cannot be in the past" });

        var task = new PomodoroTask
        {
            Title = request.Title,
            Description = request.Description,
            EstimatedPomodoros = request.EstimatedPomodoros,
            DueDate = request.DueDate,
            Status = request.Status,
            ProjectId = request.ProjectId,
            Priority = request.Priority,
            Recurrence = request.Recurrence,
            User = null
        };

        var created = await _taskRepository.CreateAsync(task, user);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> Update(Guid id, [FromBody] UpdateTaskRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var existing = await _taskRepository.GetByIdAsync(id, includeRelated: false);
        if (existing == null) return NotFound();
        if (existing.UserId != user.Id)
        {
            _logger.LogWarning("User {UserId} attempted to update task {TaskId} belonging to another user", user.Id, id);
            return Forbid();
        }

        var wasCompleted = existing.Status == TaskStatus.Completed;

        existing.Title = request.Title;
        existing.Description = request.Description;
        existing.EstimatedPomodoros = request.EstimatedPomodoros;
        existing.DueDate = request.DueDate;
        existing.Status = request.Status;
        existing.Priority = request.Priority;
        existing.Recurrence = request.Recurrence;

        var updated = await _taskRepository.UpdateAsync(existing);
        if (updated == null) return NotFound();

        if (!wasCompleted
            && updated.Status == TaskStatus.Completed
            && updated.Recurrence != RecurrencePattern.None)
        {
            await CreateRecurringTask(updated, user);
        }

        return Ok(MapToResponse(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var task = await _taskRepository.GetByIdAsync(id, includeRelated: false);
        if (task == null) return NotFound();
        if (task.UserId != user.Id) return Forbid();

        await _taskRepository.SoftDeleteAsync(id);
        return NoContent();
    }

    private async System.Threading.Tasks.Task CreateRecurringTask(PomodoroTask completed, ApplicationUser user)
    {
        var nextDueDate = completed.DueDate ?? DateTime.UtcNow;
        nextDueDate = completed.Recurrence switch
        {
            RecurrencePattern.Daily => nextDueDate.AddDays(1),
            RecurrencePattern.Weekly => nextDueDate.AddDays(7),
            RecurrencePattern.Monthly => nextDueDate.AddMonths(1),
            _ => nextDueDate
        };

        var next = new PomodoroTask
        {
            Title = completed.Title,
            Description = completed.Description,
            EstimatedPomodoros = completed.EstimatedPomodoros,
            Priority = completed.Priority,
            Recurrence = completed.Recurrence,
            ProjectId = completed.ProjectId,
            Status = TaskStatus.NotStarted,
            DueDate = nextDueDate,
            User = null
        };

        await _taskRepository.CreateAsync(next, user);
        _logger.LogInformation("Created recurring task {NewId} from completed task {CompletedId}", next.Id, completed.Id);
    }

    private static TaskResponse MapToResponse(PomodoroTask t) => new(
        t.Id, t.TaskNumber, t.Title, t.Description,
        t.EstimatedPomodoros, t.CompletedPomodoros,
        t.Status.ToString(), t.Priority.ToString(), t.Recurrence.ToString(),
        t.DueDate, t.ProjectId, t.CreatedAt, t.UpdatedAt
    );
}