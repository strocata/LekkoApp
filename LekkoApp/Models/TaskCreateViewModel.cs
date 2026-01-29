using Microsoft.AspNetCore.Mvc.Rendering;
using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.Models;

public class TaskCreateViewModel
{
    public string Title { get; set; }
    public string? Description { get; set; }

    public int EstimatedPomodoros { get; set; }
    public DateTime? DueDate { get; set; }

    public TaskStatus Status { get; set; }

    public Guid? ProjectId { get; set; }

    public Enums.Priority Priority { get; set; } = Enums.Priority.Medium;
    public Enums.RecurrencePattern Recurrence { get; set; } = Enums.RecurrencePattern.None;

    public IEnumerable<SelectListItem> Projects { get; set; } = [];
}
