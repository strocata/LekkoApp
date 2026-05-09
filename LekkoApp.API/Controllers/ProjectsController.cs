using LekkoApp.API.DTOs.Requests;
using LekkoApp.API.DTOs.Responses;
using LekkoApp.Data;
using LekkoApp.Models;
using LekkoApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LekkoApp.API.Controllers;

public class ProjectsController : BaseApiController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProjectRepository _projectRepository;

    public ProjectsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IProjectRepository projectRepository)
    {
        _context = context;
        _userManager = userManager;
        _projectRepository = projectRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> Get()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var projects = await _projectRepository.GetProjectsByUserAsync(user);
        return Ok(projects.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectResponse>> GetById(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var project = await _projectRepository.GetProjectByIdAsync(id);
        if (project == null) return NotFound();
        if (project.User?.Id != user.Id) return Forbid();

        return Ok(MapToResponse(project));
    }

    [HttpGet("{id:guid}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskSummaryResponse>>> GetTasks(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var project = await _projectRepository.GetProjectByIdAsync(id);
        if (project == null) return NotFound();
        if (project.User?.Id != user.Id) return Forbid();

        var tasks = await _projectRepository.GetProjectTasksByIdAsync(id);
        return Ok(tasks
            .Where(t => t != null)
            .Select(t => new TaskSummaryResponse(
                t!.Id, t.Title, t.Description,
                t.Status.ToString(), t.Priority.ToString(),
                t.DueDate, t.ProjectId)));
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create([FromBody] CreateProjectRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.Status,
            User = null
        };

        await _projectRepository.CreateAsync(project, user);

        if (request.SelectedTaskIds.Any())
        {
            var tasksToUpdate = await _context.PomodoroTasks
                .Where(t => request.SelectedTaskIds.Contains(t.Id))
                .ToListAsync();

            foreach (var task in tasksToUpdate)
                task.ProjectId = project.Id;

            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, MapToResponse(project));
    }

    private static ProjectResponse MapToResponse(Project p) => new(
        p.Id, p.Name, p.Description, p.StartDate, p.EndDate,
        p.IsActive, p.Status, p.User, p.CreatedAt
    );
}