using LekkoApp.Helpers;

namespace LekkoApp.Models;

public class TasksViewModel
{
    public PaginatedList<Task>? Tasks { get; set; }
    public Task? SelectedTask { get; set; }
}