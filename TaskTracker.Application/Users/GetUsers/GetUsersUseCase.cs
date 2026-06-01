using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Users.GetUsers;

public class GetUsersUseCase
{
    private readonly IUserManagementService _userManagementService;

    public GetUsersUseCase(
        IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<List<GetUsersResponse>> Execute(
        GetUsersRequest request)
    {
        return await _userManagementService
            .GetUsersAsync(request);
    }
}