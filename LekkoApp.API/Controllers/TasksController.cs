using LekkoApp.API.DTOs.Requests;
using LekkoApp.API.DTOs.Responses;
using LekkoApp.Data;
using LekkoApp.Models;
using LekkoApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LekkoApp.API.Controllers;

public class TasksController : BaseApiController
{
    private readonly TaskRepository _taskRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public TasksController(TaskRepository taskRepository, UserManager<ApplicationUser> userManager)
    {
        _taskRepository = taskRepository;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var response = await _taskRepository.GetByUserAsync(user);
        return Ok(response);
    }
}
