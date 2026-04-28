using LekkoApp.Data;
using LekkoApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LekkoApp.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TaskRepository> _logger;

    public TaskRepository(ApplicationDbContext context, ILogger<TaskRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets a task by ID with all related entities eagerly loaded
    /// </summary>
    public async Task<PomodoroTask?> GetByIdAsync(Guid id, bool includeRelated = false)
    {
        try
        {
            if (!includeRelated)
            {
                return await _context.PomodoroTasks.FindAsync(id);
            }

            return await _context.PomodoroTasks
                .Include(t => t.Project)
                .Include(t => t.Tags)
                .Include(t => t.PomodoroSessions)
                .Include(t => t.ParentTask)
                .Include(t => t.Subtasks)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task with ID {TaskId}", id);
            throw;
        }
    }

    /// <summary>
    /// Gets all tasks for a specific user
    /// </summary>
    public async Task<List<PomodoroTask>> GetByUserAsync(ApplicationUser? user, bool includeRelated = false)
    {
        try
        {
            if (user == null)
            {
                return new List<PomodoroTask>();
            }

            var query = _context.PomodoroTasks.Where(t => t.User == user);

            if (includeRelated)
            {
                query = query
                    .Include(t => t.Project)
                    .Include(t => t.Tags)
                    .Include(t => t.PomodoroSessions);
            }

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for user {UserId}", user?.Id);
            throw;
        }
    }

    /// <summary>
    /// Gets queryable tasks for a user (for filtering/sorting in controllers)
    /// Use with caution - prefer GetByUserAsync when possible
    /// </summary>
    public IQueryable<PomodoroTask> GetByUserQueryable(ApplicationUser? user)
    {
        if (user == null)
        {
            return Enumerable.Empty<PomodoroTask>().AsQueryable();
        }

        return _context.PomodoroTasks.Where(t => t.User == user);
    }

    /// <summary>
    /// Gets tasks by status for a specific user
    /// </summary>
    public async Task<List<PomodoroTask>> GetByStatusAsync(ApplicationUser? user, Models.Enums.TaskStatus status)
    {
        try
        {
            if (user == null)
            {
                return new List<PomodoroTask>();
            }

            return await _context.PomodoroTasks
                .Where(t => t.User == user && t.Status == status)
                .Include(t => t.Project)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks with status {Status} for user {UserId}",
                status, user?.Id);
            throw;
        }
    }

    /// <summary>
    /// Gets overdue tasks for a specific user
    /// </summary>
    public async Task<List<PomodoroTask>> GetOverdueTasksAsync(ApplicationUser? user)
    {
        try
        {
            if (user == null)
            {
                return new List<PomodoroTask>();
            }

            var now = DateTime.UtcNow;
            return await _context.PomodoroTasks
                .Where(t => t.User == user
                            && t.DueDate < now
                            && t.Status != Models.Enums.TaskStatus.Completed)
                .Include(t => t.Project)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue tasks for user {UserId}", user?.Id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new pomodoroTask
    /// </summary>
    public async Task<PomodoroTask> CreateAsync(PomodoroTask pomodoroTask, ApplicationUser? user)
    {
        try
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null when creating a pomodoroTask");
            }

            pomodoroTask.Id = Guid.NewGuid();
            pomodoroTask.User = user;
            pomodoroTask.CreatedAt = DateTime.UtcNow;
            pomodoroTask.UpdatedAt = DateTime.UtcNow;
            pomodoroTask.CompletedPomodoros = 0;

            await _context.PomodoroTasks.AddAsync(pomodoroTask);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created pomodoroTask {TaskId} for user {UserId}", pomodoroTask.Id, user.Id);
            return pomodoroTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pomodoroTask for user {UserId}", user?.Id);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing task
    /// </summary>
    public async Task<PomodoroTask?> UpdateAsync(PomodoroTask? updatedTask)
    {
        try
        {
            if (updatedTask == null)
            {
                return null;
            }

            var currentTask = await _context.PomodoroTasks.FindAsync(updatedTask.Id);
            if (currentTask == null)
            {
                _logger.LogWarning("Attempted to update non-existent task {TaskId}", updatedTask.Id);
                return null;
            }

            // Update fields
            currentTask.Status = updatedTask.Status;
            currentTask.Title = updatedTask.Title;
            currentTask.Description = updatedTask.Description;
            currentTask.DueDate = updatedTask.DueDate;
            currentTask.EstimatedPomodoros = updatedTask.EstimatedPomodoros;
            currentTask.Priority = updatedTask.Priority;
            currentTask.Recurrence = updatedTask.Recurrence;
            currentTask.UpdatedAt = DateTime.UtcNow;

            _context.PomodoroTasks.Update(currentTask);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated task {TaskId}", currentTask.Id);
            return currentTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId}", updatedTask?.Id);
            throw;
        }
    }

    /// <summary>
    /// Soft deletes a task by setting it inactive
    /// </summary>
    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        try
        {
            var task = await _context.PomodoroTasks.FindAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Attempted to delete non-existent task {TaskId}", id);
                return false;
            }

            // Soft delete implementation - add IsDeleted field to PomodoroTask model
            // task.IsDeleted = true;
            // task.UpdatedAt = DateTime.UtcNow;

            // For now, just remove it
            _context.PomodoroTasks.Remove(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted task {TaskId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", id);
            throw;
        }
    }

    /// <summary>
    /// Hard deletes a task from the database
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var task = await _context.PomodoroTasks.FindAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Attempted to delete non-existent task {TaskId}", id);
                return false;
            }

            _context.PomodoroTasks.Remove(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Hard deleted task {TaskId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hard deleting task {TaskId}", id);
            throw;
        }
    }

    /// <summary>
    /// Checks if a task exists
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.PomodoroTasks.AnyAsync(t => t.Id == id);
    }

    /// <summary>
    /// Gets task statistics for a user
    /// </summary>
    public async Task<TaskStatistics> GetStatisticsAsync(ApplicationUser? user)
    {
        try
        {
            if (user == null)
            {
                return new TaskStatistics();
            }

            var tasks = await _context.PomodoroTasks
                .Where(t => t.User == user)
                .ToListAsync();

            var now = DateTime.UtcNow;

            return new TaskStatistics
            {
                TotalTasks = tasks.Count,
                CompletedTasks = tasks.Count(t => t.Status == Models.Enums.TaskStatus.Completed),
                InProgressTasks = tasks.Count(t => t.Status == Models.Enums.TaskStatus.InProgress),
                NotStartedTasks = tasks.Count(t => t.Status == Models.Enums.TaskStatus.NotStarted),
                OverdueTasks = tasks.Count(t => t.DueDate < now && t.Status != Models.Enums.TaskStatus.Completed),
                CompletedPomodoros = tasks.Sum(t => t.CompletedPomodoros),
                EstimatedPomodoros = tasks.Sum(t => t.EstimatedPomodoros)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for user {UserId}", user?.Id);
            throw;
        }
    }
}

/// <summary>
/// Statistics about tasks for a user
/// </summary>
public class TaskStatistics
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int NotStartedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int CompletedPomodoros { get; set; }
    public int EstimatedPomodoros { get; set; }
}