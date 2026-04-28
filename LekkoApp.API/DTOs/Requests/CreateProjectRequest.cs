using System.ComponentModel.DataAnnotations;

namespace LekkoApp.API.DTOs.Requests;

public record CreateProjectRequest(
    [Required, MaxLength(200)] string Name,
    string? Description
);
