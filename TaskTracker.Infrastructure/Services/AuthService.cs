using Microsoft.AspNetCore.Identity;

using TaskTracker.Application.Interfaces;
using TaskTracker.Infrastructure.Identity;

namespace TaskTracker.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;

    private readonly IJwtProvider _jwtProvider;

    public AuthService(
        UserManager<AppUser> userManager,
        IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<string> LoginAsync(
        string email,
        string password)
    {
        var user =
            await _userManager.FindByEmailAsync(email);

        if (user == null)
            throw new Exception("Invalid credentials");

        var valid =
            await _userManager.CheckPasswordAsync(
                user,
                password);

        if (!valid)
            throw new Exception("Invalid credentials");

        // 🔥 roles
        var roles =
            await _userManager.GetRolesAsync(user);

        // 🔥 JWT with roles
        return _jwtProvider.GenerateToken(
            user.Id,
            user.Email!,
            roles);
    }
}