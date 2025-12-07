using LekkoApp.Data;
using LekkoApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace LekkoApp.Controllers;

public class TimerController : Controller
{
    
    private readonly ApplicationDbContext _context;


    public TimerController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(TimerViewModel model)
    {
        return View(model);
    }
}