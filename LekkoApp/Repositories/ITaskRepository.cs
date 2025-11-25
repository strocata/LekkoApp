using Task = LekkoApp.Models.Task;

namespace LekkoApp.Repositories;

public interface ITaskRepository
{
    Task<Task> GetByIdAsync(Guid taskId);
    Task<IEnumerable<Task>> GetAllAsync();
}