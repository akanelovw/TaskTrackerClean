using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Users.GetUserById;

public class GetUserByIdUseCase
{
    private readonly IUserManagementService _userManagementService;
    private readonly IUserService _userService;

    public GetUserByIdUseCase(
        IUserManagementService userManagementService, IUserService userService)
    {
        _userManagementService = userManagementService;
        _userService = userService;
    }

    public async Task<GetUserByIdResponse> Execute(
        GetUserByIdRequest request)
    {
        if (!_userService.IsInRole(Roles.Admin))
            throw new ForbiddenException();

        var user = await _userManagementService
            .GetByIdAsync(request.UserId);

        if (user == null)
            throw new NotFoundException("User not found");

        return user;
    }
}