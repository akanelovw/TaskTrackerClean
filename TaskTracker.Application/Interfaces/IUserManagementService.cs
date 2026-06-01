using TaskTracker.Application.Users.GetUsers;
using TaskTracker.Application.Users.GetUserById;

namespace TaskTracker.Application.Interfaces;

public interface IUserManagementService
{
    Task<string> CreateUserAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        string role);

    Task<List<GetUsersResponse>> GetUsersAsync(
        GetUsersRequest request);

    Task<GetUserByIdResponse?> GetByIdAsync(
        string userId);

    Task UpdateUserAsync(
        string userId,
        string firstName,
        string lastName,
        string email,
        string role);

    Task DeleteUserAsync(
        string userId);
}