using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.WorkItems.AssignUser;
using TaskTracker.Application.WorkItems.ChangeStatus;
using TaskTracker.Application.WorkItems.CreateWorkItem;
using TaskTracker.Application.WorkItems.DeleteWorkItem;
using TaskTracker.Application.WorkItems.GetWorkItems;
using TaskTracker.Application.WorkItems.UpdateWorkItem;
using TaskTracker.Api.Common;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/workitems")]
[Authorize]
public class WorkItemsController : ControllerBase
{
    private readonly CreateWorkItemUseCase _create;
    private readonly ChangeWorkItemStatusUseCase _changeStatus;
    private readonly AssignUserUseCase _assign;
    private readonly UpdateWorkItemUseCase _update;
    private readonly DeleteWorkItemUseCase _delete;
    private readonly GetWorkItemsUseCase _list;

    public WorkItemsController(
        CreateWorkItemUseCase create,
        ChangeWorkItemStatusUseCase changeStatus,
        AssignUserUseCase assign,
        UpdateWorkItemUseCase update,
        DeleteWorkItemUseCase delete,
        GetWorkItemsUseCase list)
    {
        _create = create;
        _changeStatus = changeStatus;
        _assign = assign;
        _update = update;
        _delete = delete;
        _list = list;
    }

    // ================= GET ALL =================
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetWorkItemsRequest request)
    {
        var result = await _list.Execute(request);

        return Ok(ApiResponse.Ok(result));
    }

    // ================= CREATE =================
    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkItemRequest request)
    {
        var result = await _create.Execute(request);

        return Ok(ApiResponse.Ok(result, "Work item created"));
    }

    // ================= UPDATE =================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateWorkItemRequest request)
    {
        request.Id = id;

        await _update.Execute(request);

        return Ok(ApiResponse.Ok("Work item updated"));
    }

    // ================= CHANGE STATUS =================
    [HttpPut("{id}/status")]
    public async Task<IActionResult> ChangeStatus(int id, ChangeWorkItemStatusRequest request)
    {
        request.WorkItemId = id;

        await _changeStatus.Execute(request);

        return Ok(ApiResponse.Ok("Status updated"));
    }

    // ================= ASSIGN USER =================
    [HttpPut("{id}/assign")]
    public async Task<IActionResult> Assign(int id, AssignUserRequest request)
    {
        request.WorkItemId = id;

        await _assign.Execute(request);

        return Ok(ApiResponse.Ok("User assigned"));
    }

    // ================= DELETE =================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _delete.Execute(new DeleteWorkItemRequest
        {
            Id = id
        });

        return Ok(ApiResponse.Ok("Work item deleted"));
    }
}