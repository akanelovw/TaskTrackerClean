using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Common;
using TaskTracker.Application.Auth.Login;
using TaskTracker.Application.Users.CreateUser;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly CreateUserUseCase _register;
    private readonly LoginUseCase _login;

    public AuthController(CreateUserUseCase register, LoginUseCase login)
    {
        _register = register;
        _login = login;
    }

    // ================= LOGIN =================
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await _login.Execute(request);

        return Ok(ApiResponse.Ok(token));
    }
}