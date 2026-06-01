using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Common;
using TaskTracker.Application.Users.CreateUser;
using TaskTracker.Application.Users.DeleteUser;
using TaskTracker.Application.Users.GetUserById;
using TaskTracker.Application.Users.GetUsers;
using TaskTracker.Application.Users.UpdateUser;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly GetUsersUseCase _getUsers;
    private readonly GetUserByIdUseCase _getById;
    private readonly CreateUserUseCase _create;
    private readonly UpdateUserUseCase _update;
    private readonly DeleteUserUseCase _delete;

    public UsersController(
        GetUsersUseCase getUsers,
        GetUserByIdUseCase getById,
        CreateUserUseCase create,
        UpdateUserUseCase update,
        DeleteUserUseCase delete)
    {
        _getUsers = getUsers;
        _getById = getById;
        _create = create;
        _update = update;
        _delete = delete;
    }

    // ================= GET ALL =================
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetUsersRequest request)
    {
        var result = await _getUsers.Execute(request);

        return Ok(ApiResponse.Ok(result));
    }

    // ================= GET BY ID =================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _getById.Execute(new GetUserByIdRequest
        {
            UserId = id
        });

        return Ok(ApiResponse.Ok(result));
    }

    // ================= CREATE =================
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var result = await _create.Execute(request);

        return Ok(ApiResponse.Ok(result, "User created"));
    }

    // ================= UPDATE =================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateUserRequest request)
    {
        request.UserId = id;

        await _update.Execute(request);

        return Ok(ApiResponse.Ok("User updated"));
    }

    // ================= DELETE =================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _delete.Execute(new DeleteUserRequest
        {
            UserId = id
        });

        return Ok(ApiResponse.Ok("User deleted"));
    }
}