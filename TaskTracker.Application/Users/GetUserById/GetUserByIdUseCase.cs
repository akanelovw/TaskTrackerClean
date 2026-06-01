using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Users.GetUserById;

public class GetUserByIdUseCase
{
    private readonly IUserManagementService _userManagementService;

    public GetUserByIdUseCase(
        IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<GetUserByIdResponse> Execute(
        GetUserByIdRequest request)
    {
        var user = await _userManagementService
            .GetByIdAsync(request.UserId);

        if (user == null)
            throw new NotFoundException("User not found");

        return user;
    }
}