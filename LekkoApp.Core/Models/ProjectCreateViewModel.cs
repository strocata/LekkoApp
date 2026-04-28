using LekkoApp.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LekkoApp.Models;

public class ProjectCreateViewModel
{
    public string Name { get; set; }
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ProjectStatus Status { get; set; }

    public IEnumerable<SelectListItem> PomodoroTasks { get; set; } = [];

    public List<Guid> SelectedTaskIds { get; set; } = [];
}