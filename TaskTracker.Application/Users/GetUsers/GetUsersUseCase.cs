using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Users.GetUsers;

public class GetUsersUseCase
{
    private readonly IUserManagementService _userManagementService;
    private readonly IUserService _userService;

    public GetUsersUseCase(
        IUserManagementService userManagementService, IUserService userService)
    {
        _userManagementService = userManagementService;
        _userService = userService;
    }

    public async Task<List<GetUsersResponse>> Execute(
        GetUsersRequest request)
    {
        if (!_userService.IsInRole(Roles.Admin) &&
            !_userService.IsInRole(Roles.ChiefProjectManager) &&
            !_userService.IsInRole(Roles.ProjectManager))
            throw new ForbiddenException();

        return await _userManagementService
            .GetUsersAsync(request);
    }
}