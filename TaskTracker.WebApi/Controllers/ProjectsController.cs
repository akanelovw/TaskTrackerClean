using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Projects.AddProjectMember;
using TaskTracker.Application.Projects.AssignProjectManager;
using TaskTracker.Application.Projects.ChangeProjectStatus;
using TaskTracker.Application.Projects.CreateProject;
using TaskTracker.Application.Projects.DeleteProject;
using TaskTracker.Application.Projects.GetProjectDetails;
using TaskTracker.Application.Projects.GetProjectsList;
using TaskTracker.Application.Projects.RemoveProjectMember;
using TaskTracker.Application.Projects.UpdateProject;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly CreateProjectUseCase _create;
    private readonly GetProjectsListUseCase _list;
    private readonly GetProjectDetailsUseCase _details;
    private readonly UpdateProjectUseCase _update;
    private readonly DeleteProjectUseCase _delete;
    private readonly AssignProjectManagerUseCase _assignManager;
    private readonly ChangeProjectStatusUseCase _changeProjectStatus;
    private readonly AddProjectMemberUseCase _addMember;
    private readonly RemoveProjectMemberUseCase _removeMember;

    public ProjectsController(
        CreateProjectUseCase create,
        GetProjectsListUseCase list,
        GetProjectDetailsUseCase details,
        UpdateProjectUseCase update,
        DeleteProjectUseCase delete,
        AssignProjectManagerUseCase assignManager,
        ChangeProjectStatusUseCase changeProjectStatus,
        AddProjectMemberUseCase addMember,
        RemoveProjectMemberUseCase removeMember)
    {
        _create = create;
        _list = list;
        _details = details;
        _update = update;
        _delete = delete;

        _assignManager = assignManager;
        _changeProjectStatus = changeProjectStatus;
        _addMember = addMember;
        _removeMember = removeMember;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _list.Execute();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _details.Execute(id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProjectRequest request)
    {
        var result = await _create.Execute(request);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProjectRequest request)
    {
        request.Id = id;

        await _update.Execute(request);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _delete.Execute(
            new DeleteProjectRequest
            {
                Id = id
            });

        return Ok();
    }

    [HttpPut("{projectId}/manager")]
    public async Task<IActionResult> AssignManager(
        int projectId,
        AssignProjectManagerRequest request)
    {
        request.ProjectId = projectId;

        await _assignManager.Execute(request);

        return Ok();
    }

    [HttpPost("{projectId}/members")]
    public async Task<IActionResult> AddMember(
        int projectId,
        AddProjectMemberRequest request)
    {
        request.ProjectId = projectId;

        await _addMember.Execute(request);

        return Ok();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> ChangeStatus(
    int id,
    ChangeProjectStatusRequest request)
    {
        request.ProjectId = id;

        await _changeProjectStatus.Execute(request);

        return Ok();
    }

    [HttpDelete("{projectId}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(
        int projectId,
        string userId)
    {
        await _removeMember.Execute(
            new RemoveProjectMemberRequest
            {
                ProjectId = projectId,
                UserId = userId
            });

        return Ok();
    }
}