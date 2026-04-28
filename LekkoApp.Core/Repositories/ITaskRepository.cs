using LekkoApp.Data;
using LekkoApp.Models;

namespace LekkoApp.Repositories;

public interface ITaskRepository
{
    Task<PomodoroTask?> GetByIdAsync(Guid id, bool includeRelated = true);
    Task<List<PomodoroTask>> GetByUserAsync(ApplicationUser? user, bool includeRelated = false);
    IQueryable<PomodoroTask> GetByUserQueryable(ApplicationUser? user);
    Task<List<PomodoroTask>> GetByStatusAsync(ApplicationUser? user, Models.Enums.TaskStatus status);
    Task<List<PomodoroTask>> GetOverdueTasksAsync(ApplicationUser? user);
    Task<PomodoroTask> CreateAsync(PomodoroTask pomodoroTask, ApplicationUser? user);
    Task<PomodoroTask?> UpdateAsync(PomodoroTask? updatedTask);
    Task<bool> SoftDeleteAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<TaskStatistics> GetStatisticsAsync(ApplicationUser? user);
}