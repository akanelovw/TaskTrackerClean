using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.WorkItems.ChangePriority;

public class ChangeWorkItemPriorityUseCase
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserService _userService;

    public ChangeWorkItemPriorityUseCase(
        IWorkItemRepository workItemRepository,
        IProjectRepository projectRepository,
        IUserService userService)
    {
        _workItemRepository = workItemRepository;
        _projectRepository = projectRepository;
        _userService = userService;
    }

    public async Task Execute(ChangeWorkItemPriorityRequest request)
    {
        var userId = _userService.GetCurrentUserId();

        var workItem = await _workItemRepository.GetByIdAsync(request.WorkItemId);

        if (workItem == null)
            throw new NotFoundException("Work item not found");

        var project = await _projectRepository.GetByIdAsync(workItem.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        if (_userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager))
        {
            workItem.ChangePriority(request.Priority);

            await _workItemRepository.UpdateAsync(workItem);
            return;
        }

        if (_userService.IsInRole(Roles.ProjectManager))
        {
            if (project.ManagerUserId != userId)
                throw new ForbiddenException();

            workItem.ChangePriority(request.Priority);

            await _workItemRepository.UpdateAsync(workItem);
            return;
        }

        if (workItem.AssignedUserId != userId)
            throw new ForbiddenException();

        workItem.ChangePriority(request.Priority);

        await _workItemRepository.UpdateAsync(workItem);
    }
}