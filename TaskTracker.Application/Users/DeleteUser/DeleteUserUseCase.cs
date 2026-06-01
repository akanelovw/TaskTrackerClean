using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Users.DeleteUser;

public class DeleteUserUseCase
{
    private readonly IUserManagementService _userManagementService;

    public DeleteUserUseCase(
        IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task Execute(DeleteUserRequest request)
    {
        await _userManagementService
            .DeleteUserAsync(request.UserId);
    }
}