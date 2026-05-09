using LekkoApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LekkoApp.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var result = await _signInManager.PasswordSignInAsync(
            req.Email, req.Password, req.RememberMe, lockoutOnFailure: true);

        if (result.IsLockedOut)  return StatusCode(423, new { error = "locked_out" });
        if (result.IsNotAllowed) return StatusCode(403, new { error = "email_not_confirmed" });
        if (!result.Succeeded)   return Unauthorized(new { error = "invalid_credentials" });

        var user = await _userManager.FindByEmailAsync(req.Email);
        return Ok(new { id = user!.Id, email = user.Email });
    }

    [HttpPost("logout"), Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }

    [HttpGet("me"), Authorize]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        return user is null
            ? Unauthorized()
            : Ok(new { id = user.Id, email = user.Email });
    }
}

public record LoginRequest(string Email, string Password, bool RememberMe = false);