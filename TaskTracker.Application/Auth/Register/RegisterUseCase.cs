using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Auth.Register;

public class RegisterUseCase
{
    private readonly IAuthService _authService;

    public RegisterUseCase(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<RegisterResponse> Execute(RegisterRequest request)
    {
        var userId = await _authService.RegisterAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        return new RegisterResponse
        {
            UserId = userId
        };
    }
}