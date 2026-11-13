using LekkoApp.API.DTOs.Responses;
using LekkoApp.Core.Services;
using LekkoApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LekkoApp.API.Controllers;

public class DashboardController : BaseApiController
{
    private readonly IDashboardService _dashboardService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(IDashboardService dashboardService,
        UserManager<ApplicationUser> userManager)
    {
        _dashboardService = dashboardService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Get(CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var data = await _dashboardService.GetForUserAsync(user.Id, ct);
        return Ok(data);
    }
}