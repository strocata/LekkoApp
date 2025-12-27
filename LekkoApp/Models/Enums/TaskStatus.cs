using System.ComponentModel.DataAnnotations;

namespace LekkoApp.Models.Enums;

public enum TaskStatus
{
    [Display(Name = "Not Started")]
    NotStarted = 0,
    [Display(Name = "In Progress")]
    InProgress = 1,
    [Display(Name = "Done")]
    Completed = 2,
    [Display(Name = "On Hold")]
    OnHold = 3,
    [Display(Name = "Cancelled")]
    Cancelled = 4
}