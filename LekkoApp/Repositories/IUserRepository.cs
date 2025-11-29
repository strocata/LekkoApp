using System.Security.Claims;
using LekkoApp.Data;

namespace LekkoApp.Repositories;

public interface IUserRepository
{
    public Task<ApplicationUser> GetUserAsync();
}