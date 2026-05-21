using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Auth.Login;
using TaskTracker.Application.Auth.Register;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUseCase _register;
    private readonly LoginUseCase _login;

    public AuthController(RegisterUseCase register, LoginUseCase login)
    {
        _register = register;
        _login = login;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _register.Execute(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await _login.Execute(request);
        return Ok(token);
    }
}