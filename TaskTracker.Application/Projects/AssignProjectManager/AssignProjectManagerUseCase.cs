using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Projects.AssignProjectManager;

public class AssignProjectManagerUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserService _userService;

    public AssignProjectManagerUseCase(
        IProjectRepository projectRepository,
        IUserService userService)
    {
        _projectRepository = projectRepository;
        _userService = userService;
    }

    public async Task Execute(AssignProjectManagerRequest request)
    {
        var currentUserId = _userService.GetCurrentUserId();

        var project = await _projectRepository.GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        if (!_userService.IsInRole(Roles.Admin) &&
            !_userService.IsInRole(Roles.ChiefProjectManager))
        {
            throw new ForbiddenException();
        }
        if (!project.HasMember(request.UserId))
            throw new ValidationException("User is not project member");

        project.ChangeManager(request.UserId);

        await _projectRepository.UpdateAsync(project);
    }
}