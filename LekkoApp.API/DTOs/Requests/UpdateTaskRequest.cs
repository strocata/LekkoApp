using System.ComponentModel.DataAnnotations;
using LekkoApp.Models.Enums;
using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.API.DTOs.Requests;

public record UpdateTaskRequest(
    [Required, MaxLength(50)] string Title,
    string? Description,
    int EstimatedPomodoros,
    DateTime? DueDate,
    TaskStatus Status,
    Priority Priority,
    RecurrencePattern Recurrence
);