using LekkoApp.Data;
using LekkoApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LekkoApp.Controllers;

public class ProjectController: Controller
{
    private readonly ILogger<TasksController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly List<Task> _userTasks;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TaskRepository _taskRepository;

    public ProjectController(ILogger<TasksController> logger, ApplicationDbContext context,
        TaskRepository taskRepository, UserManager<ApplicationUser> userManager)
    {
        _taskRepository = taskRepository;
        _userManager = userManager;
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }
}