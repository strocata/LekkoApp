using LekkoApp.Models;
using Microsoft.AspNetCore.Mvc;
using LekkoApp.Data;
using LekkoApp.Helpers;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TaskRepository _taskRepository;


        public TasksController(ILogger<TasksController> logger,
            TaskRepository taskRepository, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _taskRepository = taskRepository;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index(Guid taskId, string sortOrder, string searchString, string currentFilter,
            int? pageNumber, int pageSize = 10)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            IQueryable<Task> userTasks = _taskRepository.GetByUser(user);
            int numberOfAllTasks = userTasks.Count();
            var selectedTask = await _taskRepository.GetByIdAsync(taskId);

            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["CreationDateSortParm"] = sortOrder == "CreationDate" ? "creation_date_desc" : "CreationDate";
            ViewData["TaskNumberSortParm"] = sortOrder == "TaskNumber" ? "task_number_desc" : "TaskNumber";
            ViewData["DueDateSortParm"] = sortOrder == "DueDate" ? "due_date_desc" : "DueDate";
            ViewData["SearchString"] = searchString;
            ViewData["PageSize"] = pageSize;
            ViewData["NumberOfAllTasks"] = numberOfAllTasks;

            if (!String.IsNullOrEmpty(searchString))
            {
                var searchString1 = searchString;
                userTasks = userTasks.Where(s => s.Title.Contains(searchString1)
                                                 || s.TaskNumber.ToString() == searchString1);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            switch (sortOrder)
            {
                case "title_desc":
                    userTasks = userTasks.OrderByDescending(s => s.Title);
                    break;
                case "CreationDate":
                    userTasks = userTasks.OrderBy(s => s.CreatedAt);
                    break;
                case "creation_date_desc":
                    userTasks = userTasks.OrderByDescending(s => s.CreatedAt);
                    break;
                case "TaskNumber":
                    userTasks = userTasks.OrderBy(s => s.TaskNumber);
                    break;
                case "task_number_desc":
                    userTasks = userTasks.OrderByDescending(s => s.TaskNumber);
                    break;
                case "DueDate":
                    userTasks = userTasks.OrderBy(s => s.DueDate);
                    break;
                case "due_date_desc":
                    userTasks = userTasks.OrderByDescending(s => s.DueDate);
                    break;
                default:
                    userTasks = userTasks.OrderBy(s => s.TaskNumber);
                    break;
            }

            var model = new TasksViewModel
            {
                Tasks = await PaginatedList<Task>.CreateAsync(userTasks.AsNoTracking(), pageNumber ?? 1, pageSize),
                SelectedTask = selectedTask
            };

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Task task)
        {
            _logger.LogInformation("Create POST invoked. Title={Title}", task.Title);

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
        public async Task<RedirectToActionResult> UpdateCardTask(Task? selectedTask)
        {
            selectedTask = await _taskRepository.Update(selectedTask);

            return RedirectToAction("Index", new { selectedId = selectedTask!.Id });
        }

        [Authorize]
        public async Task<IActionResult> StartTimer(Guid taskId)
        {
            var selectedTask = await _taskRepository.GetByIdAsync(taskId);

            var session = new TimerViewModel
            {
                SelectedTask = selectedTask
            };
            return View("~/Views/Timer/Index.cshtml", session);
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid taskId)
        {
            return View(await _taskRepository.GetByIdAsync(taskId));
        }
    }
}