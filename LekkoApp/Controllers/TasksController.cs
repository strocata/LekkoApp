using LekkoApp.Models;
using Microsoft.AspNetCore.Mvc;
using LekkoApp.Data;
using Microsoft.AspNetCore.Authorization;
using LekkoApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task = LekkoApp.Models.Task;

namespace LekkoApp.Controllers
{
    public class TasksController : Controller
    {
        private readonly ILogger<TasksController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly List<Task> _userTasks;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TaskRepository _taskRepository;
        
        
        public TasksController(ILogger<TasksController> logger, ApplicationDbContext context, TaskRepository taskRepository, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _taskRepository = taskRepository;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index(Guid taskId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userTasks = await _taskRepository.GetByUserAsync(user);
            var selectedTask = await _taskRepository.GetByIdAsync(taskId);
            
            var model = new TasksViewModel
            {
                Tasks = userTasks,
                SelectedTask = selectedTask
            };

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var model =  new Models.Task { EstimatedPomodoros = 1, Status = Models.TaskStatus.NotStarted };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Task task)
        {
            _logger.LogInformation("Create POST invoked. Title={Title}", task?.Title);

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    var key = entry.Key;
                    var errors = entry.Value.Errors;

                    foreach (var error in errors)
                    {
                        _logger.LogInformation($"Property: {key}, Error: {error.ErrorMessage}");
                    }
                }

                _logger.LogWarning("ModelState invalid");
                return View(task);
            }
            
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);

            await _taskRepository.Create(task, user);

            return RedirectToAction("Create");
        }

        [Authorize]
        [HttpPost] 
        public async Task<RedirectToActionResult> UpdateCardTask(Task selectedTask)
        {
            selectedTask = await _taskRepository.Update(selectedTask);
            
            return RedirectToAction("Index", new { selectedId = selectedTask.Id });
        }
        
        [Authorize]
        public async Task<IActionResult> StartTimer(Guid taskId)
        {
            var selectedTask = await _taskRepository.GetByIdAsync(taskId);
            
            var session = new TimerViewModel
            {
                SelectedTask = selectedTask
            };
            return View("~/Views/Timer/Index.cshtml",  session);
        }
    }
}