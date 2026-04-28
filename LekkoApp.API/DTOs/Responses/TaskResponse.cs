namespace LekkoApp.API.DTOs.Responses;

public record TaskResponse(
    int Id,
    string Title,
    string? Description,
    int ProjectId,
    DateTime? DueDate,
    bool IsCompleted
);
