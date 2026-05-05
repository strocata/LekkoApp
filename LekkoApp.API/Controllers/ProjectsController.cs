using LekkoApp.API.DTOs.Responses;
using LekkoApp.Data;
using LekkoApp.Models;
using LekkoApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LekkoApp.API.Controllers;

public class ProjectsController: BaseApiController
{
    private readonly ILogger<ProjectsController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public ProjectsController(ILogger<ProjectsController> logger, ApplicationDbContext context,
        ITaskRepository taskRepository, UserManager<ApplicationUser> userManager, IProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _userManager = userManager;
        _logger = logger;
        _context = context;
        _projectRepository = projectRepository;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> Get()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return Unauthorized();

        var projects = await _projectRepository.GetProjectsByUserAsync(user);
        var response = projects.Select(p => new ProjectResponse(
            p.Id, p.Name, p.Description, p.StartDate, p.EndDate,
            p.IsActive, p.Status, p.User, p.CreatedAt
        )).ToList();

        return Ok(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(ProjectCreateViewModel model)
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

            return Ok(model);
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

        return Ok();
    }
}