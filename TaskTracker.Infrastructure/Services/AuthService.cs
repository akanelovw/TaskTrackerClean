using Microsoft.AspNetCore.Identity;
using TaskTracker.Application.Common.Exceptions;
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
        {
            throw new NotFoundException($"User - {email} not found");
        }

        var valid =
            await _userManager.CheckPasswordAsync(
                user,
                password);

        if (!valid)
        {
            throw new UnauthorizedException(
                $"Invalid password");
        }


        var roles =
            await _userManager.GetRolesAsync(user);

        return _jwtProvider.GenerateToken(
            user.Id,
            user.Email!,
            roles);
    }
}