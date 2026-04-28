using LekkoApp.Models;
using Microsoft.AspNetCore.Mvc;
using LekkoApp.Data;
using LekkoApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using LekkoApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace LekkoApp.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ILogger<TasksController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TaskRepository _taskRepository;
        private readonly ProjectRepository _projectRepository;

        public TasksController(
            ILogger<TasksController> logger,
            TaskRepository taskRepository, 
            UserManager<ApplicationUser> userManager,
            ProjectRepository projectRepository)
        {
            _logger = logger;
            _taskRepository = taskRepository;
            _userManager = userManager;
            _projectRepository = projectRepository;
        }

        public async Task<IActionResult> Index(
            Guid? taskId, 
            string? sortOrder, 
            string? searchString, 
            string? currentFilter,
            int? pageNumber, 
            int pageSize = 10)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return Challenge();
                }

                // Get queryable for filtering/sorting
                IQueryable<PomodoroTask> userTasks = _taskRepository.GetByUserQueryable(user)
                    .Where(t => !t.IsDeleted); // Exclude soft-deleted tasks

                int numberOfAllTasks = await userTasks.CountAsync();
                
                PomodoroTask? selectedTask = null;
                if (taskId.HasValue)
                {
                    selectedTask = await _taskRepository.GetByIdAsync(taskId.Value);
                    if (selectedTask?.User?.Id != user.Id)
                    {
                        _logger.LogWarning("User {UserId} attempted to access task {TaskId} belonging to another user", 
                            user.Id, taskId);
                        selectedTask = null; // Don't show tasks from other users
                    }
                }

                // ViewData for sorting and filtering
                ViewData["CurrentSort"] = sortOrder;
                ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
                ViewData["CreationDateSortParm"] = sortOrder == "CreationDate" ? "creation_date_desc" : "CreationDate";
                ViewData["TaskNumberSortParm"] = sortOrder == "TaskNumber" ? "task_number_desc" : "TaskNumber";
                ViewData["DueDateSortParm"] = sortOrder == "DueDate" ? "due_date_desc" : "DueDate";
                ViewData["PrioritySortParm"] = sortOrder == "Priority" ? "priority_desc" : "Priority";
                ViewData["UpdatedAtSortParm"] = sortOrder == "UpdatedAt" ? "updated_at_desc" : "UpdatedAt";
                ViewData["SearchString"] = searchString;
                ViewData["PageSize"] = pageSize;
                ViewData["NumberOfAllTasks"] = numberOfAllTasks;

                // Search filtering
                if (!String.IsNullOrEmpty(searchString))
                {
                    userTasks = userTasks.Where(s => 
                        s.Title.Contains(searchString) || 
                        s.TaskNumber.ToString() == searchString ||
                        (s.Description != null && s.Description.Contains(searchString)));
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                // Sorting
                userTasks = sortOrder switch
                {
                    "title_desc" => userTasks.OrderByDescending(s => s.Title),
                    "CreationDate" => userTasks.OrderBy(s => s.CreatedAt),
                    "creation_date_desc" => userTasks.OrderByDescending(s => s.CreatedAt),
                    "TaskNumber" => userTasks.OrderBy(s => s.TaskNumber),
                    "task_number_desc" => userTasks.OrderByDescending(s => s.TaskNumber),
                    "DueDate" => userTasks.OrderBy(s => s.DueDate),
                    "due_date_desc" => userTasks.OrderByDescending(s => s.DueDate),
                    "Priority" => userTasks.OrderBy(s => s.Priority),
                    "priority_desc" => userTasks.OrderByDescending(s => s.Priority),
                    "UpdatedAt" => userTasks.OrderBy(s => s.UpdatedAt),
                    "updated_at_desc" => userTasks.OrderByDescending(s => s.UpdatedAt),
                    _ => userTasks.OrderBy(s => s.TaskNumber)
                };

                var model = new PomodoroTasksViewModel
                {
                    PomodoroTasks = await PaginatedList<PomodoroTask>.CreateAsync(
                        userTasks.AsNoTracking(), 
                        pageNumber ?? 1, 
                        pageSize),
                    SelectedPomodoroTask = selectedTask
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tasks index");
                TempData["ErrorMessage"] = "An error occurred while loading tasks. Please try again.";
                return View(new PomodoroTasksViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return Challenge();
                }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading task creation form");
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskCreateViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return Challenge();
                }

                if (!ModelState.IsValid)
                {
                    // Reload dropdown
                    model.Projects = (await _projectRepository.GetProjectsByUserAsync(user))
                        .Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.Name
                        });

                    return View(model);
                }

                // Additional validation
                if (model.DueDate.HasValue && model.DueDate.Value < DateTime.UtcNow)
                {
                    ModelState.AddModelError(nameof(model.DueDate), "Due date cannot be in the past");
                    model.Projects = (await _projectRepository.GetProjectsByUserAsync(user))
                        .Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.Name
                        });
                    return View(model);
                }

                var task = new PomodoroTask
                {
                    Title = model.Title,
                    Description = model.Description,
                    EstimatedPomodoros = model.EstimatedPomodoros,
                    DueDate = model.DueDate,
                    Status = model.Status,
                    ProjectId = model.ProjectId,
                    Priority = model.Priority,
                    Recurrence = model.Recurrence,
                    User = null // Will be set in repository
                };

                await _taskRepository.CreateAsync(task, user);

                TempData["SuccessMessage"] = $"PomodoroTask '{task.Title}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                ModelState.AddModelError("", "An error occurred while creating the task. Please try again.");
                
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user != null)
                {
                    model.Projects = (await _projectRepository.GetProjectsByUserAsync(user))
                        .Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.Name
                        });
                }
                
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCardTask(PomodoroTask? selectedTask)
        {
            try
            {
                if (selectedTask == null)
                {
                    TempData["ErrorMessage"] = "PomodoroTask not found.";
                    return RedirectToAction(nameof(Index));
                }

                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return Challenge();
                }

                var oldTask = await _taskRepository.GetByIdAsync(selectedTask.Id, includeRelated: false);
                
                if (oldTask == null)
                {
                    TempData["ErrorMessage"] = "PomodoroTask not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Verify ownership
                if (oldTask.User?.Id != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to update task {TaskId} belonging to another user", 
                        user.Id, selectedTask.Id);
                    return Forbid();
                }

                var wasCompleted = oldTask.Status == Models.Enums.TaskStatus.Completed;

                selectedTask = await _taskRepository.UpdateAsync(selectedTask);

                // Handle recurring tasks
                if (selectedTask != null
                    && !wasCompleted 
                    && selectedTask.Status == Models.Enums.TaskStatus.Completed
                    && selectedTask.Recurrence != Models.Enums.RecurrencePattern.None)
                {
                    await CreateRecurringTask(selectedTask, user);
                }

                TempData["SuccessMessage"] = "PomodoroTask updated successfully!";
                return RedirectToAction(nameof(Index), new { taskId = selectedTask?.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                TempData["ErrorMessage"] = "An error occurred while updating the task.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task CreateRecurringTask(PomodoroTask completedPomodoroTask, ApplicationUser user)
        {
            var nextDueDate = completedPomodoroTask.DueDate ?? DateTime.UtcNow;

            nextDueDate = completedPomodoroTask.Recurrence switch
            {
                Models.Enums.RecurrencePattern.Daily => nextDueDate.AddDays(1),
                Models.Enums.RecurrencePattern.Weekly => nextDueDate.AddDays(7),
                Models.Enums.RecurrencePattern.Monthly => nextDueDate.AddMonths(1),
                _ => nextDueDate
            };

            var newTask = new PomodoroTask
            {
                Title = completedPomodoroTask.Title,
                Description = completedPomodoroTask.Description,
                EstimatedPomodoros = completedPomodoroTask.EstimatedPomodoros,
                Priority = completedPomodoroTask.Priority,
                Recurrence = completedPomodoroTask.Recurrence,
                ProjectId = completedPomodoroTask.ProjectId,
                Status = Models.Enums.TaskStatus.NotStarted,
                DueDate = nextDueDate,
                User = null
            };

            await _taskRepository.CreateAsync(newTask, user);
            _logger.LogInformation("Created recurring pomodoroTask {TaskId} from completed pomodoroTask {CompletedTaskId}", 
                newTask.Id, completedPomodoroTask.Id);
        }

        public async Task<IActionResult> StartTimer(Guid taskId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return Challenge();
                }

                var selectedTask = await _taskRepository.GetByIdAsync(taskId);
                
                if (selectedTask == null)
                {
                    TempData["ErrorMessage"] = "PomodoroTask not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Verify ownership
                if (selectedTask.User?.Id != user.Id)
                {
                    return Forbid();
                }

                var session = new TimerViewModel
                {
                    SelectedTask = selectedTask
                };
                
                return View("~/Views/Timer/Index.cshtml", session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting timer for task {TaskId}", taskId);
                TempData["ErrorMessage"] = "An error occurred while starting the timer.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid taskId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return Challenge();
                }

                var task = await _taskRepository.GetByIdAsync(taskId);
                
                if (task == null)
                {
                    TempData["ErrorMessage"] = "PomodoroTask not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Verify ownership
                if (task.User?.Id != user.Id)
                {
                    return Forbid();
                }

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for task {TaskId}", taskId);
                TempData["ErrorMessage"] = "An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return Challenge();
                }

                var task = await _taskRepository.GetByIdAsync(id, includeRelated: false);
                
                if (task == null)
                {
                    TempData["ErrorMessage"] = "PomodoroTask not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Verify ownership
                if (task.User?.Id != user.Id)
                {
                    return Forbid();
                }

                var success = await _taskRepository.SoftDeleteAsync(id);
                
                if (success)
                {
                    TempData["SuccessMessage"] = $"PomodoroTask '{task.Title}' deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete task.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the task.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}