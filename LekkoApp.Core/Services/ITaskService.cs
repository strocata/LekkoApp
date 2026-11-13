using LekkoApp.Models;

namespace LekkoApp.Core.Services;

public interface ITaskService
{
    Task<PomodoroTask?> GetByIdAsync(Guid id);
    Task<PomodoroTask> CreateAsync(PomodoroTask task);
    Task UpdateAsync(PomodoroTask task);
    Task DeleteAsync(PomodoroTask task);
}