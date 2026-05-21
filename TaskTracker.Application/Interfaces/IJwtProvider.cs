namespace TaskTracker.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(string userId, string email);
}