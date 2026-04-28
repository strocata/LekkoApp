using System.Diagnostics;
using LekkoApp.Data;
using LekkoApp.Models;
using LekkoApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LekkoApp.Controllers;

public class ProjectsController : Controller
{
    private readonly ILogger<ProjectsController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TaskRepository _taskRepository;
    private readonly ProjectRepository _projectRepository;

    public ProjectsController(ILogger<ProjectsController> logger, ApplicationDbContext context,
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

        var model = new ProjectsViewModel
        {
            Projects = projects,
            ProjectsCount = projects.Count()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var tasks = await _taskRepository.GetByUserAsync(user);

        var model = new ProjectCreateViewModel
        {
            PomodoroTasks = tasks.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Title
            }),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectCreateViewModel model)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);

        if (!ModelState.IsValid)
        {
            var tasks = await _taskRepository.GetByUserAsync(user);
            model.PomodoroTasks = tasks.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Title
            });

            return View(model);
        }

        var project = new Project
        {
            Name = model.Name,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Status = model.Status,
            User = null,
        };

        await _projectRepository.CreateAsync(project, user);

        if (model.SelectedTaskIds.Any())
        {
            var tasksToUpdate = await _context.PomodoroTasks
                .Where(t => model.SelectedTaskIds.Contains(t.Id))
                .ToListAsync();

            foreach (var task in tasksToUpdate)
            {
                task.ProjectId = project.Id;
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(Guid projectId)
    {
        var project = await _projectRepository.GetProjectByIdAsync(projectId);

        var tasks = await _projectRepository.GetProjectTasksByIdAsync(projectId) as List<PomodoroTask>;

        var model = new ProjectDetailsViewModel()
        {
            Project = project,
            PomodoroTasks = tasks,
            DonePomodoroCount = tasks.Select(item => item.CompletedPomodoros).Sum(),
            EstimatedPomodoroCount = tasks.Select(item => item.EstimatedPomodoros).Sum()
        };

        return View(model);
    }
}