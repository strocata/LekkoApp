namespace LekkoApp.Helpers;
using LekkoApp.Models.Enums;

public static class BadgeStyles
{
    private static readonly Dictionary<TaskStatus, string> Map = new()
    {
        { TaskStatus.NotStarted, "bg-blue text-blue-fg" },
        { TaskStatus.Completed,  "bg-green text-green-fg" },
        { TaskStatus.Cancelled,  "bg-red text-red-fg" },
        { TaskStatus.InProgress, "bg-yellow text-yellow-fg" },
        { TaskStatus.OnHold,     "bg-orange text-orange-fg" },
    };

    public static string Get(TaskStatus status)
        => Map[status];
}