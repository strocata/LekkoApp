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

    public IEnumerable<SelectListItem> Projects { get; set; } = [];
}
