using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LekkoApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace LekkoApp.Controllers;

public class DashboardController: Controller
{
    private readonly ILogger<DashboardController> _logger;
    
    public DashboardController(ILogger<DashboardController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }
}