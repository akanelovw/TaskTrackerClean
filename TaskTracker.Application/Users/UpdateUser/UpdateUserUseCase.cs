using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Users.UpdateUser;

public class UpdateUserUseCase
{
    private readonly IUserManagementService _userManagementService;
    private readonly IUserService _userService;

    public UpdateUserUseCase(
        IUserManagementService userManagementService, IUserService userService)
    {
        _userManagementService = userManagementService;
        _userService = userService;
    }

    public async Task Execute(UpdateUserRequest request)
    {
        if (!_userService.IsInRole(Roles.Admin))
            throw new ForbiddenException();

        await _userManagementService.UpdateUserAsync(
            request.UserId,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Role);
    }
}