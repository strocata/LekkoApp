using LekkoApp.Data;
using Task = LekkoApp.Models.Task;
namespace LekkoApp.Repositories;

public class TaskRepository : ITaskRepository 
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Task?> GetAsync(Task? task)
    {
        if (task != null)
        {
            var selectedTask  = await _context.Tasks.FindAsync(task.Id);
            return selectedTask;
        }
        return null;
    }

    public async Task<Task?> GetByIdAsync(Guid id)
    {
        var selectedTask = await _context.Tasks.FindAsync(id);
        return selectedTask;
    }

    public IQueryable<Task> GetByUser(ApplicationUser? user)
    {
        var selectedTasks =  _context.Tasks.Where(t => t.User == user);
        return selectedTasks;
    }

    public async Task<Task> Create(Task task, ApplicationUser? user)
    {
        task.Id = Guid.NewGuid();
        task.User = user;
        task.CreatedAt = DateTime.UtcNow;
        task.CompletedPomodoros = 0;

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<Task?> Update(Task? updatedTask)
    {
        var currentTask = await _context.Tasks.FindAsync(updatedTask.Id);
        if (currentTask != null)
        {
            currentTask.Status = updatedTask.Status;
            currentTask.Title = updatedTask.Title;
            currentTask.Description = updatedTask.Description;
            _context.Tasks.Update(currentTask);
            await _context.SaveChangesAsync();
        }

        return currentTask;
    }
}