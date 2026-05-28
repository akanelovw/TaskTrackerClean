using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Projects.RemoveProjectMember;

public class RemoveProjectMemberUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserService _userService;

    public RemoveProjectMemberUseCase(
        IProjectRepository projectRepository,
        IUserService userService)
    {
        _projectRepository = projectRepository;
        _userService = userService;
    }

    public async Task Execute(RemoveProjectMemberRequest request)
    {
        var currentUserId = _userService.GetCurrentUserId();

        var project = await _projectRepository.GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        if (_userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager))
        {
            project.RemoveMember(request.UserId);

            await _projectRepository.UpdateAsync(project);
            return;
        }

        if (_userService.IsInRole(Roles.ProjectManager))
        {
            if (project.ManagerUserId != currentUserId)
                throw new ForbiddenException();

            project.RemoveMember(request.UserId);

            await _projectRepository.UpdateAsync(project);
            return;
        }

        throw new ForbiddenException();
    }
}