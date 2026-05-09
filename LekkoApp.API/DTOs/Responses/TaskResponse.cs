namespace LekkoApp.API.DTOs.Responses;

public record TaskResponse(
    Guid Id,
    int TaskNumber,
    string Title,
    string? Description,
    int EstimatedPomodoros,
    int CompletedPomodoros,
    string Status,
    string Priority,
    string Recurrence,
    DateTime? DueDate,
    Guid? ProjectId,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);