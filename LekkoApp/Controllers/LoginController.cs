using Microsoft.AspNetCore.Mvc;

namespace LekkoApp.Controllers;

public class LoginController: Controller
{
    private  readonly ILogger<LoginController> _logger;
    
    public LoginController(ILogger<LoginController> logger)
    {
        _logger = logger;
    }
    
    public IActionResult Index()
    {
        return View();
    }
}