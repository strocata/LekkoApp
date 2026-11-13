using LekkoApp.Models.Enums;

namespace LekkoApp.API.DTOs.Responses;

public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    ProjectStatus Status,
    DateTime CreatedAt
);