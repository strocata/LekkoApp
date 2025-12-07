using System.Security.Claims;
using LekkoApp.Data;
using Microsoft.AspNetCore.Mvc;
using Task = LekkoApp.Models.Task;

namespace LekkoApp.Repositories;

public interface ITaskRepository
{
    Task<Task?> GetAsync(Task task);
    Task<Task?> GetByIdAsync(Guid id);
    Task<List<Task>> GetByUserAsync(ApplicationUser user);
    
    Task<Task> Create(Task task, ApplicationUser user);
    Task<Task> Update(Task task);
    
}