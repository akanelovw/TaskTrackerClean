namespace TaskTracker.Application.Interfaces;

public interface IUserManagementService
{
    Task<string> CreateUserAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        string role);
}