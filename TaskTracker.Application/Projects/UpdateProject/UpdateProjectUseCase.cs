using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Projects.UpdateProject;

public class UpdateProjectUseCase
{
    private readonly IProjectRepository _repo;
    private readonly IUserService _userService;

    public UpdateProjectUseCase(IProjectRepository repo, IUserService userService)
    {
        _repo = repo;
        _userService = userService;
    }

    public async Task Execute(UpdateProjectRequest request)
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

        project.Update(
            request.Title,
            request.CustomerCompany,
            request.ExecutorCompany,
            request.StartTime,
            request.EndTime,
            request.Priority
            
        );

        await _repo.UpdateAsync(project);
    }
}