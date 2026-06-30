namespace TaskTracker.Maui.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Role { get; }
    bool IsInRole(string role);
}