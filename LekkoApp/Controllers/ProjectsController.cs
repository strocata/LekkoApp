using LekkoApp.Data;
using LekkoApp.Models;
using LekkoApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LekkoApp.Controllers;

public class ProjectsController: Controller
{
    private readonly ILogger<TasksController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TaskRepository _taskRepository;
    private readonly ProjectRepository _projectRepository;

    public ProjectsController(ILogger<TasksController> logger, ApplicationDbContext context,
        TaskRepository taskRepository, UserManager<ApplicationUser> userManager, ProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _userManager = userManager;
        _logger = logger;
        _context = context;
        _projectRepository = projectRepository;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);

        List<Project> projects = await _projectRepository.GetProjectsByUserAsync(user) as List<Project>;
        
        var model = new ProjectViewModel
        {
            Projects = projects,
            ProjectsCount = projects.Count()
        };
        
        return View(model);
    }
    
}