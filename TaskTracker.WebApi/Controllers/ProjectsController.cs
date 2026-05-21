using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Projects.CreateProject;
using TaskTracker.Application.Projects.GetProjectsList;
using TaskTracker.Application.Projects.GetProjectDetails;
using TaskTracker.Application.Projects.UpdateProject;
using TaskTracker.Application.Projects.DeleteProject;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly CreateProjectUseCase _create;
    private readonly GetProjectsListUseCase _list;
    private readonly GetProjectDetailsUseCase _details;
    private readonly UpdateProjectUseCase _update;
    private readonly DeleteProjectUseCase _delete;

    public ProjectsController(
        CreateProjectUseCase create,
        GetProjectsListUseCase list,
        GetProjectDetailsUseCase details,
        UpdateProjectUseCase update,
        DeleteProjectUseCase delete)
    {
        _create = create;
        _list = list;
        _details = details;
        _update = update;
        _delete = delete;
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
    public async Task<IActionResult> Create(CreateProjectRequest request)
    {
        var id = await _create.Execute(request);

        return Ok(id);
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
}