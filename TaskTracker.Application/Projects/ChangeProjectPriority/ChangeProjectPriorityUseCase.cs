using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Projects.ChangeProjectPriority;

public class ChangeProjectPriorityUseCase
{
    private readonly IProjectRepository _repo;
    private readonly IUserService _userService;

    public ChangeProjectPriorityUseCase(
        IProjectRepository repo,
        IUserService userService)
    {
        _repo = repo;
        _userService = userService;
    }

    public async Task Execute(ChangeProjectPriorityRequest request)
    {
        var project = await _repo.GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        var isAllowed =
            _userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager);

        if (!isAllowed)
            throw new ForbiddenException();

        project.ChangePriority(request.Priority);

        await _repo.UpdateAsync(project);
    }
}