using LekkoApp.Core.Services.Models;

namespace LekkoApp.Core.Services;

public interface IDashboardService
{
    Task<DashboardData> GetForUserAsync(string userId, CancellationToken ct = default);
}