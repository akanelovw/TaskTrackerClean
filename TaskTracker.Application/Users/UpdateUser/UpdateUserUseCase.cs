using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Users.UpdateUser;

public class UpdateUserUseCase
{
    private readonly IUserManagementService _userManagementService;

    public UpdateUserUseCase(
        IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task Execute(UpdateUserRequest request)
    {
        await _userManagementService.UpdateUserAsync(
            request.UserId,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Role);
    }
}