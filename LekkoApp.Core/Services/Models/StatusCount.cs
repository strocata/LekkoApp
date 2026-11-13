using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.Core.Services.Models;

public record StatusCount(TaskStatus Status, int Count);
