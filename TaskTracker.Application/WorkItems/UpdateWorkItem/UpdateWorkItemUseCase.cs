using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.WorkItems.UpdateWorkItem;

public class UpdateWorkItemUseCase
{
    private readonly IWorkItemRepository _repo;
    private readonly IUserService _userService;
    private readonly IProjectRepository _projectRepository;

    public UpdateWorkItemUseCase(
        IWorkItemRepository repo,
        IUserService userService,
        IProjectRepository projectRepository)
    {
        _repo = repo;
        _userService = userService;
        _projectRepository = projectRepository;
    }

    public async Task Execute(UpdateWorkItemRequest request)
    {
        var userId = _userService.GetCurrentUserId();

        var item = await _repo.GetByIdAsync(request.Id);

        if (item == null)
            throw new NotFoundException("Work item not found");

        var project = await _projectRepository.GetByIdAsync(item.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        if (!_userService.IsInRole(Roles.Admin) &&
            !_userService.IsInRole(Roles.ChiefProjectManager))
        {
            if (_userService.IsInRole(Roles.ProjectManager))
            {
                if (project.ManagerUserId != userId)
                    throw new ForbiddenException();
            }
            else
            {
                if (item.AssignedUserId != userId)
                    throw new ForbiddenException();
            }
        }

        item.UpdateTitle(request.Title);
        item.UpdateComment(request.Comment);
        item.ChangePriority(request.Priority);

        await _repo.UpdateAsync(item);
    }
}