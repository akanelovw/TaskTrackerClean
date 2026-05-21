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

    public async Task<string> RegisterAsync(
        string email,
        string password,
        string firstName,
        string lastName)
    {
        var exists = await _userManager.FindByEmailAsync(email);

        if (exists != null)
            throw new Exception("User already exists");

        var user = new AppUser
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new Exception(
                string.Join(", ", result.Errors.Select(x => x.Description)));
        }

        return user.Id;
    }

    public async Task<string> LoginAsync(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            throw new Exception("Invalid credentials");

        var valid = await _userManager.CheckPasswordAsync(
            user,
            password);

        if (!valid)
            throw new Exception("Invalid credentials");

        return _jwtProvider.GenerateToken(
            user.Id,
            user.Email!);
    }

}