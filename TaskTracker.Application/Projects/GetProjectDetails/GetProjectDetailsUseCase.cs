using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Common.Mappings;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Projects.GetProjectDetails;

public class GetProjectDetailsUseCase
{
    private readonly IProjectRepository _repo;
    private readonly IUserService _userService;

    public GetProjectDetailsUseCase(
        IProjectRepository repo,
        IUserService userService)
    {
        _repo = repo;
        _userService = userService;
    }

    public async Task<GetProjectDetailsResponse> Execute(int id)
    {
        var project = await _repo.GetByIdAsync(id);

        if (project == null)
            throw new NotFoundException("Project not found");

        var userId = _userService.GetCurrentUserId();

        if (_userService.IsInRole(Roles.Admin))
            return ProjectMapping.ToDetails(project);

        if (_userService.IsInRole(Roles.ChiefProjectManager))
            return ProjectMapping.ToDetails(project);

        if (_userService.IsInRole(Roles.ProjectManager))
        {
            if (project.ManagerUserId != userId)
                throw new ForbiddenException();

            return ProjectMapping.ToDetails(project);
        }

        if (!project.HasMember(userId))
            throw new ForbiddenException();

        return ProjectMapping.ToDetails(project);
    }
}