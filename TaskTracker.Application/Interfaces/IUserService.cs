namespace TaskTracker.Application.Interfaces;

public interface IUserService
{
    string GetCurrentUserId();

    Task<bool> ExistsAsync(string userId);

    bool IsInRole(string role);
}