using LekkoApp.API.DTOs.Requests;
using LekkoApp.API.DTOs.Responses;
using LekkoApp.Data;
using LekkoApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LekkoApp.API.Controllers;

public class TagsController : BaseApiController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TagsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagResponse>>> Get()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var tags = await _context.Tags
            .Where(t => t.UserId == user.Id)
            .OrderBy(t => t.Name)
            .Select(t => new TagResponse(t.Id, t.Name, t.Color))
            .ToListAsync();

        return Ok(tags);
    }

    [HttpPost]
    public async Task<ActionResult<TagResponse>> Create([FromBody] CreateTagRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Color = request.Color,
            UserId = user.Id
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        var response = new TagResponse(tag.Id, tag.Name, tag.Color);
        return CreatedAtAction(nameof(Get), response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

        if (tag == null) return NotFound();

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}