using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Auth.Login;

public class LoginUseCase
{
    private readonly IAuthService _authService;

    public LoginUseCase(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<LoginResponse> Execute(LoginRequest request)
    {
        var token = await _authService.LoginAsync(
            request.Email,
            request.Password);

        return new LoginResponse
        {
            Token = token
        };
    }
}