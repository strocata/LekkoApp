using LekkoApp.Data;
using Task = LekkoApp.Models.Task;

namespace LekkoApp.Repositories;

public interface ITaskRepository
{
    Task<Task?> GetAsync(Task task);
    Task<Task?> GetByIdAsync(Guid id);
    IQueryable<Task> GetByUser(ApplicationUser user);
    
    Task<Task> Create(Task task, ApplicationUser user);
    Task<Task?> Update(Task task);
    
}