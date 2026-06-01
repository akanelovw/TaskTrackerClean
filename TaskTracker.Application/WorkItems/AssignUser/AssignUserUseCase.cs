using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.WorkItems.AssignUser;

public class AssignUserUseCase
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserService _userService;

    public AssignUserUseCase(
        IWorkItemRepository workItemRepository,
        IProjectRepository projectRepository,
        IUserService userService)
    {
        _workItemRepository = workItemRepository;
        _projectRepository = projectRepository;
        _userService = userService;
    }

    public async Task Execute(AssignUserRequest request)
    {
        var currentUserId = _userService.GetCurrentUserId();

        var workItem = await _workItemRepository.GetByIdAsync(request.WorkItemId);

        if (workItem == null)
            throw new NotFoundException("Work item not found");

        var project = await _projectRepository.GetByIdAsync(workItem.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        if (_userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager))
        {
            workItem.AssignUser(request.UserId);

            await _workItemRepository.UpdateAsync(workItem);
            return;
        }

        if (_userService.IsInRole(Roles.ProjectManager))
        {
            if (project.ManagerUserId != currentUserId)
                throw new ForbiddenException();

            if (!project.HasMember(request.UserId))
                throw new BadRequestException("User is not project member");

            workItem.AssignUser(request.UserId);

            await _workItemRepository.UpdateAsync(workItem);
            return;
        }

        throw new ForbiddenException();
    }
}