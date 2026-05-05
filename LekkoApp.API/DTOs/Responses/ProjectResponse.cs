using LekkoApp.Data;
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
    ApplicationUser? User,
    DateTime CreatedAt
);