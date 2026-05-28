using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Projects.GetProjectDetails;

public class GetProjectDetailsUseCase
{
    private readonly IProjectRepository _repo;
    private readonly IUserService _userService;

    public GetProjectDetailsUseCase(IProjectRepository repo, IUserService userService)
    {
        _repo = repo;
        _userService = userService;
    }

    public async Task<Project> Execute(int id)
    {

        var project = await _repo.GetByIdAsync(id);

        if (project == null)
        {
            throw new NotFoundException("Project not found");
        }

        var userId =
            _userService.GetCurrentUserId();

        if (_userService.IsInRole(Roles.Admin))
        {
            return project;
        }

        if (_userService.IsInRole(
            Roles.ChiefProjectManager))
        {
            return project;
        }

        if (_userService.IsInRole(
            Roles.ProjectManager))
        {
            if (project.ManagerUserId != userId)
            {
                throw new ForbiddenException();
            }

            return project;
        }

        if (!project.HasMember(userId))
        {
            throw new ForbiddenException();
        }

        return project;
    }
}