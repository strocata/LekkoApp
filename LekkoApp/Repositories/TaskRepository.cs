using Task = LekkoApp.Models.Task;

namespace LekkoApp.Repositories;

public class TaskRepository: ITaskRepository
{
    public Task<Task> GetByIdAsync(Guid taskId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Task>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}