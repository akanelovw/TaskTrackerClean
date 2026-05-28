using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Projects.DeleteProject;

public class DeleteProjectUseCase
{
    private readonly IProjectRepository _repo;
    private readonly IUserService _userService;

    public DeleteProjectUseCase(IProjectRepository repo, IUserService userService)
    {
        _repo = repo;
        _userService = userService;
    }

    public async Task Execute(DeleteProjectRequest request)
    {
        var project = await _repo.GetByIdAsync(request.Id);

        if (project == null)
        {
            throw new NotFoundException("Project not found");
        }

        var userId =
            _userService.GetCurrentUserId();

        if (!_userService.IsInRole(Roles.Admin) &&
            !_userService.IsInRole(
                Roles.ChiefProjectManager))
        {
            if (project.ManagerUserId != userId)
            {
                throw new ForbiddenException();
            }
        }

        await _repo.DeleteAsync(project);

    }
}