using System.Net;
using System.Security.Claims;
using LekkoApp.Data;
using Microsoft.AspNetCore.Identity;

namespace LekkoApp.Repositories;

public class UserRepository: IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    private readonly UserManager<ApplicationUser> _userManager;
    
    public UserRepository(ApplicationDbContext context,  UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    public Task<ApplicationUser> GetUserAsync()
    {
        throw new NotImplementedException();
    }
}