namespace TaskTracker.Application.Interfaces;

public interface IUserRoleService
{
    Task<bool> IsInRoleAsync(string userId, string role);
}