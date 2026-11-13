using LekkoApp.Models;
using LekkoApp.Data;

namespace LekkoApp.Repositories;

public interface IProjectRepository
{
    public Task<IEnumerable<Project>> GetProjectsByUserAsync(ApplicationUser user);
    public Task<Project?> GetProjectByIdAsync(Guid id);
    public Task<Project?> GetProjectByIdForUserAsync(Guid id, string userId);
    public Task<IEnumerable<PomodoroTask?>> GetProjectTasksByIdAsync(Guid id);
    public Task<Project> CreateAsync(Project project, ApplicationUser user);
}