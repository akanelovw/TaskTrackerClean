using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Users.GetUserById;
using TaskTracker.Application.Users.GetUsers;
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

    public async Task<string> CreateUserAsync(
    string email,
    string password,
    string firstName,
    string lastName,
    string role)
    {
        var exists = await _userManager.FindByEmailAsync(email);

        if (exists != null)
            throw new BadRequestException("User already exists");

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
            throw new BadRequestException(
                string.Join(", ", result.Errors.Select(x => x.Description)));
        }

        await _userManager.AddToRoleAsync(user, role);

        return user.Id;
    }

    public async Task<List<GetUsersResponse>> GetUsersAsync(
        GetUsersRequest request)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.Email!.Contains(request.Search) ||
                x.FirstName.Contains(request.Search) ||
                x.LastName.Contains(request.Search));
        }

        query = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        var users = await query.ToListAsync();

        var result = new List<GetUsersResponse>();

        foreach (var user in users)
        {
            var roles =
                await _userManager.GetRolesAsync(user);

            result.Add(new GetUsersResponse
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roles.FirstOrDefault() ?? ""
            });
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            result = result
                .Where(x => x.Role == request.Role)
                .ToList();
        }

        return result;
    }

    public async Task<GetUserByIdResponse?> GetByIdAsync(
        string userId)
    {
        var user =
            await _userManager.FindByIdAsync(userId);

        if (user == null)
            return null;

        var roles =
            await _userManager.GetRolesAsync(user);

        return new GetUserByIdResponse
        {
            Id = user.Id,
            UserName = user.UserName ?? "",
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName ?? "",
            FullName = user.FullName,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? ""
        };
    }

    public async Task UpdateUserAsync(
        string userId,
        string firstName,
        string lastName,
        string email,
        string role)
    {
        var user =
            await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new NotFoundException(
                "User not found");

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        user.UserName = email;

        var updateResult =
            await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            throw new BadRequestException(
                string.Join(", ",
                    updateResult.Errors
                        .Select(x => x.Description)));
        }

        var currentRoles =
            await _userManager.GetRolesAsync(user);

        await _userManager.RemoveFromRolesAsync(
            user,
            currentRoles);

        await _userManager.AddToRoleAsync(
            user,
            role);
    }

    public async Task DeleteUserAsync(
        string userId)
    {
        var user =
            await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new NotFoundException(
                "User not found");

        var result =
            await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            throw new BadRequestException(
                string.Join(", ",
                    result.Errors
                        .Select(x => x.Description)));
        }
    }
}