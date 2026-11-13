using LekkoApp.Data;
using LekkoApp.Models;
using LekkoApp.Repositories;
using Microsoft.AspNetCore.Identity;

namespace LekkoApp.Core.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly UserManager<ApplicationUser> _userManager;


    public TaskService(ITaskRepository taskRepository, UserManager<ApplicationUser> userManager)
    {
        _taskRepository = taskRepository;
        _userManager = userManager;
    }

    // public Task<PomodoroTask?> GetByIdAsync(Guid id)
    // {
    //     var task = await _taskRepository.GetByIdAsync(id);
    //     if (task == null) return NotFound();
    //     if (task.UserId != user.Id) return Forbid();
    // }

    private Task<PomodoroTask?> NotFound()
    {
        throw new NotImplementedException();
    }

    public Task<PomodoroTask?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<PomodoroTask> CreateAsync(PomodoroTask task)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(PomodoroTask task)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(PomodoroTask task)
    {
        throw new NotImplementedException();
    }
}