using LekkoApp.Models;
using LekkoApp.Data;

namespace LekkoApp.Repositories;

public interface IProjectRepository
{
    public Task<IEnumerable<Project>> GetProjectsByUserAsync(ApplicationUser user);
    public Task<Project?> GetProjectByIdAsync(Guid id);
}