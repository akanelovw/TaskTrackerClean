using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Users.CreateUser;

public class CreateUserUseCase
{
    private readonly IUserManagementService _userManagementService;
    private readonly IUserService _userService;

    public CreateUserUseCase(
        IUserManagementService userManagementService,
        IUserService userService)
    {
        _userManagementService = userManagementService;
        _userService = userService;
    }

    public async Task<CreateUserResponse> Execute(CreateUserRequest request)
    {
        if (!_userService.IsInRole(Roles.Admin))
            throw new ForbiddenException();

        var userId = await _userManagementService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Role);

        return new CreateUserResponse
        {
            UserId = userId
        };
    }
}