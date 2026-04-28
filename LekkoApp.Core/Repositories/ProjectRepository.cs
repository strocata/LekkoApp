using LekkoApp.Data;
using LekkoApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LekkoApp.Repositories;

public class ProjectRepository: IProjectRepository
{
    
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Project>> GetProjectsByUserAsync(ApplicationUser user)
    {
        return await _context.Projects
            .Where(p => p.User.Id.ToString() == user.Id)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(Guid id)
    {
        return await _context.Projects.FindAsync(id);
    }

    public async Task<IEnumerable<PomodoroTask?>> GetProjectTasksByIdAsync(Guid id)
    {
        return await _context.PomodoroTasks.Where(t => t.ProjectId == id).ToListAsync();
    }

    public async Task<Project> CreateAsync(Project project, ApplicationUser user)
    {
        
        project.Id = Guid.NewGuid();
        project.User = user;
        project.CreatedAt = DateTime.UtcNow;

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
        return project;
    }

}