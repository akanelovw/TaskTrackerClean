using Microsoft.AspNetCore.Identity;
using TaskTracker.Application.Interfaces;
using TaskTracker.Infrastructure.Identity;

namespace TaskTracker.Infrastructure.Services;

public class UserRoleService : IUserRoleService
{
    private readonly UserManager<AppUser> _userManager;

    public UserRoleService(
        UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> IsInRoleAsync(
        string userId,
        string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return false;

        return await _userManager.IsInRoleAsync(user, role);
    }
}