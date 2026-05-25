using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Users.CreateUser;

public class CreateUserUseCase
{
    private readonly IUserManagementService _userManagementService;

    public CreateUserUseCase(
        IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<CreateUserResponse> Execute(
        CreateUserRequest request)
    {
        await _userManagementService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Role);

        return new CreateUserResponse();
    }
}