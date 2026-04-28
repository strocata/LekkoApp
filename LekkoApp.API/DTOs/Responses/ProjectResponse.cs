namespace LekkoApp.API.DTOs.Responses;

public record ProjectResponse(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt
);
