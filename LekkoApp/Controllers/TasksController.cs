using LekkoApp.Models;
using Microsoft.AspNetCore.Mvc;
using LekkoApp.Data;
using LekkoApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using LekkoApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Task = LekkoApp.Models.Task;

namespace LekkoApp.Controllers
{
    public class TasksController : Controller
    {
        private readonly ILogger<TasksController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TaskRepository _taskRepository;
        private readonly ProjectRepository _projectRepository;


        public TasksController(ILogger<TasksController> logger,
            TaskRepository taskRepository, UserManager<ApplicationUser> userManager,
            ProjectRepository projectRepository)
        {
            _logger = logger;
            _taskRepository = taskRepository;
            _userManager = userManager;
            _projectRepository = projectRepository;
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
            ViewData["PrioritySortParm"] = sortOrder == "Priority" ? "priority_desc" : "Priority";
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
                case "Priority":
                    userTasks = userTasks.OrderBy(s => s.Priority);
                    break;
                case "priority_desc":
                    userTasks = userTasks.OrderByDescending(s => s.Priority);
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
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var projects = await _projectRepository.GetProjectsByUserAsync(user);

            var vm = new TaskCreateViewModel
            {
                Projects = projects.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskCreateViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (!ModelState.IsValid)
            {
                // reload dropdown
                model.Projects = (await _projectRepository.GetProjectsByUserAsync(user))
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    });

                return View(model);
            }

            var task = new Task
            {
                Title = model.Title,
                Description = model.Description,
                EstimatedPomodoros = model.EstimatedPomodoros,
                DueDate = model.DueDate,
                Status = model.Status,
                ProjectId = model.ProjectId,
                Priority = model.Priority,
                Recurrence = model.Recurrence,
                User = null
            };

            await _taskRepository.Create(task, user);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        public async Task<RedirectToActionResult> UpdateCardTask(Task? selectedTask)
        {
            if (selectedTask == null)
            {
                return RedirectToAction("Index");
            }

            var oldTask = await _taskRepository.GetByIdAsync(selectedTask.Id);
            var wasCompleted = oldTask?.Status == Models.Enums.TaskStatus.Completed;

            selectedTask = await _taskRepository.Update(selectedTask);

            if (selectedTask != null
                && wasCompleted == false
                && selectedTask.Status == Models.Enums.TaskStatus.Completed
                && selectedTask.Recurrence != Models.Enums.RecurrencePattern.None)
            {
                var nextDueDate = selectedTask.DueDate ?? DateTime.UtcNow;

                switch (selectedTask.Recurrence)
                {
                    case Models.Enums.RecurrencePattern.Daily:
                        nextDueDate = nextDueDate.AddDays(1);
                        break;
                    case Models.Enums.RecurrencePattern.Weekly:
                        nextDueDate = nextDueDate.AddDays(7);
                        break;
                    case Models.Enums.RecurrencePattern.Monthly:
                        nextDueDate = nextDueDate.AddMonths(1);
                        break;
                }

                var newTask = new Task
                {
                    Title = selectedTask.Title,
                    Description = selectedTask.Description,
                    EstimatedPomodoros = selectedTask.EstimatedPomodoros,
                    Priority = selectedTask.Priority,
                    Recurrence = selectedTask.Recurrence,
                    ProjectId = selectedTask.ProjectId,
                    Status = Models.Enums.TaskStatus.NotStarted,
                    DueDate = nextDueDate,
                    User = null
                };

                var user = await _userManager.GetUserAsync(HttpContext.User);
                await _taskRepository.Create(newTask, user);
            }

            return RedirectToAction("Index", new { selectedId = selectedTask?.Id });
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