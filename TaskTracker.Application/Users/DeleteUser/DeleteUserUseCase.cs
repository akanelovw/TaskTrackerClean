using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Users.DeleteUser;

public class DeleteUserUseCase
{
    private readonly IUserManagementService _userManagementService;
    private readonly IUserService _userService;

    public DeleteUserUseCase(
        IUserManagementService userManagementService, IUserService userService)
    {
        _userManagementService = userManagementService;
        _userService = userService;
    }

    public async Task Execute(DeleteUserRequest request)
    {
        if (!_userService.IsInRole(Roles.Admin))
            throw new ForbiddenException();

        await _userManagementService
            .DeleteUserAsync(request.UserId);
    }
}