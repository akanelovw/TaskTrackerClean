using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using TaskTracker.Application.Interfaces;
using TaskTracker.Infrastructure.Identity;

namespace TaskTracker.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly UserManager<AppUser> _userManager;

    public UserService(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public string GetCurrentUserId()
    {
        var userId = _httpContextAccessor
            .HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            throw new Exception("User is unauthorized");

        return userId;
    }

    public async Task<bool> ExistsAsync(string userId)
    {
        var user =
            await _userManager.FindByIdAsync(userId);

        return user != null;
    }

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?
            .User?
            .IsInRole(role) ?? false;
    }
}