using System.ComponentModel.DataAnnotations;

namespace LekkoApp.API.DTOs.Requests;

public record CreateTagRequest(
    [Required, MaxLength(50)] string Name,
    [MaxLength(20)] string Color = "#206bc4"
);