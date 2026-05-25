using Microsoft.AspNetCore.Identity;

using TaskTracker.Application.Interfaces;
using TaskTracker.Infrastructure.Identity;

namespace TaskTracker.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<AppUser> _userManager;

    public UserManagementService(
        UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task CreateUserAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        string role)
    {
        var exists =
            await _userManager.FindByEmailAsync(email);

        if (exists != null)
            throw new Exception("User already exists");

        var user = new AppUser
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result =
            await _userManager.CreateAsync(
                user,
                password);

        if (!result.Succeeded)
        {
            throw new Exception(
                string.Join(", ",
                    result.Errors.Select(x => x.Description)));
        }

        await _userManager.AddToRoleAsync(
            user,
            role);
    }
}