using System.ComponentModel.DataAnnotations;

namespace LekkoApp.API.DTOs.Requests;

public record CreateTaskRequest(
    [Required, MaxLength(300)] string Title,
    string? Description,
    int ProjectId,
    DateTime? DueDate
);
