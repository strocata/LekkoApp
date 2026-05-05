using System.ComponentModel.DataAnnotations;
using LekkoApp.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LekkoApp.API.DTOs.Requests;

public record CreateProjectRequest(
    [Required, MaxLength(200)] string Name,
    DateTime StartDate,
    DateTime EndDate,
    ProjectStatus Status,
    List<Guid> SelectedTaskIds,
    string? Description
)
{
    public IEnumerable<SelectListItem> PomodoroTasks { get; set; } = Enumerable.Empty<SelectListItem>();
}
