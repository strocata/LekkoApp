using LekkoApp.Data;
using LekkoApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LekkoApp.Controllers;

[Authorize]
public class TagsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TagsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var tags = await _context.Tags
            .Where(t => t.UserId == user!.Id)
            .OrderBy(t => t.Name)
            .ToListAsync();
        return View(tags);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Tag tag)
    {
        var user = await _userManager.GetUserAsync(User);

        // Remove validation for User and Tasks
        ModelState.Remove(nameof(Tag.User));
        ModelState.Remove(nameof(Tag.UserId));
        ModelState.Remove(nameof(Tag.Tasks));

        if (ModelState.IsValid)
        {
            tag.Id = Guid.NewGuid();
            tag.UserId = user!.Id;
            // tag.User = user; // Not strictly needed for Insert, EF uses UserId

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(tag);
    }

    [HttpPost]
    // Usually Delete is via POST to prevent CSRF, or Delete method
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user!.Id);

        if (tag != null)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
