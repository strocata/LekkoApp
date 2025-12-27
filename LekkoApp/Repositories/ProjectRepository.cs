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
            .Where(p => p.UserId.ToString() == user.Id)
            .ToListAsync();
    }

}