using LekkoApp.Data;
using LekkoApp.Models;
using Microsoft.EntityFrameworkCore;
using Task = LekkoApp.Models.Task;

namespace LekkoApp.Repositories;

public class ProjectRepository: IProjectRepository
{
    
    private readonly ApplicationDbContext _context;
    private IProjectRepository _projectRepositoryImplementation;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Project>> GetProjectsByUserAsync(ApplicationUser user)
    {
        return await _context.Projects
            .Where(p => p.UserId.ToString() == user.Id)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(Guid id)
    {
        return await _context.Projects.FindAsync(id);
    }

    public async Task<IEnumerable<Task?>> GetProjectTasksByIdAsync(Guid id)
    {
        return await _context.Tasks.Where(t => t.ProjectId == id).ToListAsync();
    }

}