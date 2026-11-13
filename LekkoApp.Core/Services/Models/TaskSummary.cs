using LekkoApp.Models.Enums;
using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.Core.Services.Models;

public record TaskSummary(
    Guid Id, string Title, string? Description,
    TaskStatus Status, Priority Priority,
    DateTime? DueDate, Guid? ProjectId);