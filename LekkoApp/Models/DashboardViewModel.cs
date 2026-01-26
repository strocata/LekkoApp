namespace LekkoApp.Models;

public class DashboardViewModel
{
    public int TotalProjects { get; set; }

    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int OverdueTasks { get; set; }

    public int PomodorosToday { get; set; }
    public int PomodorosThisWeek { get; set; }
    public double AvgPomodorosPerDay { get; set; }

    public List<StatItem> TasksByStatus { get; set; } = [];
    public List<DailyStat> PomodorosByDay { get; set; } = [];

    public List<Task> OverdueTaskList { get; set; } = [];
    public List<Task> ActiveTasks { get; set; } = [];
}

public class StatItem
{
    public string Label { get; set; }
    public int Value { get; set; }
}

public class DailyStat
{
    public DateTime Date { get; set; }
    public int Value { get; set; }
}

