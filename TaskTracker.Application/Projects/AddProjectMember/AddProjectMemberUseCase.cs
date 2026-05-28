using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Projects.AddProjectMember;

public class AddProjectMemberUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserService _userService;

    public AddProjectMemberUseCase(
        IProjectRepository projectRepository,
        IUserService userService)
    {
        _projectRepository = projectRepository;
        _userService = userService;
    }

    public async Task Execute(AddProjectMemberRequest request)
    {
        var currentUserId = _userService.GetCurrentUserId();

        var project = await _projectRepository.GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        if (_userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager))
        {
            project.AddMember(request.UserId);

            await _projectRepository.UpdateAsync(project);
            return;
        }

        if (_userService.IsInRole(Roles.ProjectManager))
        {
            if (project.ManagerUserId != currentUserId)
                throw new ForbiddenException();

            project.AddMember(request.UserId);

            await _projectRepository.UpdateAsync(project);
            return;
        }

        throw new ForbiddenException();
    }
}