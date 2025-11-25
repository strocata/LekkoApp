using LekkoApp.Models;
using Microsoft.AspNetCore.Mvc;
using LekkoApp.Data;
using Microsoft.AspNetCore.Authorization;
using Task = LekkoApp.Models.Task;

namespace LekkoApp.Controllers
{
    public class TasksController : Controller
    {
        private readonly ILogger<TasksController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly List<Task> _tasks;
        
        
        public TasksController(ILogger<TasksController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _tasks = _context.Tasks.ToList();
        }

        [Authorize]
        public IActionResult Index(Guid? selectedId)
        {
            var selectedTask = selectedId.HasValue
                ? _tasks.FirstOrDefault(i => i.Id == selectedId.Value)
                : null;

            var model = new TasksViewModel
            {
                Tasks = _tasks,
                SelectedTask = selectedTask
            };

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            // optional: send defaults
            var model = new Models.Task { EstimatedPomodoros = 1, Status = Models.TaskStatus.NotStarted };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Models.Task task)
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

            // set server-only fields:
            task.Id = Guid.NewGuid();
            task.UserId = Guid.NewGuid();
            task.CreatedAt = DateTime.UtcNow;
            task.CompletedPomodoros = 0;

            // save to DB...
            _context.Tasks.Add(task);
            _context.SaveChanges();

            return RedirectToAction("Create");
        }

        [Authorize]
        [HttpPost] 
        public IActionResult UpdateCardTask(Models.Task selectedTask)
        {
            var item = _tasks.FirstOrDefault(i => i.Id == selectedTask.Id);
            if (item != null)
            {
                item.Status = selectedTask.Status;
                item.Title = selectedTask.Title;
                item.Description = selectedTask.Description;
                _context.Tasks.Update(item);
                _context.SaveChanges();
            }
            
            return RedirectToAction("Index", new { selectedId = selectedTask.Id });
        }
        
        [Authorize]
        public IActionResult StartTimer(TasksViewModel model)
        {
            var selectedTask = _tasks.FirstOrDefault(i => i.Id == model.SelectedTask.Id);
            
            var session = new TimerViewModel
            {
                SelectedTask = selectedTask
            };
            return View("~/Views/Timer/Index.cshtml",  session);
        }
    }
}