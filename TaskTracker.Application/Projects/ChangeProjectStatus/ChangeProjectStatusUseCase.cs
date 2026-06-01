using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Projects.ChangeProjectStatus;

public class ChangeProjectStatusUseCase
{
    private readonly IProjectRepository _repo;
    private readonly IUserService _userService;

    public ChangeProjectStatusUseCase(
        IProjectRepository repo,
        IUserService userService)
    {
        _repo = repo;
        _userService = userService;
    }

    public async Task Execute(ChangeProjectStatusRequest request)
    {
        var project = await _repo.GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        var isAllowed =
            _userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager);

        if (!isAllowed)
            throw new ForbiddenException();

        project.ChangeStatus(request.Status);

        await _repo.UpdateAsync(project);
    }
}