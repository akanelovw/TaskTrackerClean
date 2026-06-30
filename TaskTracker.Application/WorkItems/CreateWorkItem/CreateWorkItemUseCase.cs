using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.WorkItems.CreateWorkItem;

public class CreateWorkItemUseCase
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserService _userService;

    public CreateWorkItemUseCase(
        IWorkItemRepository workItemRepository,
        IUserService userService,
        IProjectRepository projectRepository)
    {
        _workItemRepository = workItemRepository;
        _userService = userService;
        _projectRepository = projectRepository;
    }
    public async Task<int> Execute(CreateWorkItemRequest request)
    {
        var userId =
            _userService.GetCurrentUserId();

        var project =
            await _projectRepository.GetByIdAsync(
                request.ProjectId);

        if (project == null)
        {
            throw new NotFoundException(
                "Project not found");
        }

        if (!_userService.IsInRole(Roles.Admin) && !_userService.IsInRole(Roles.ChiefProjectManager))
        {
            if (project.ManagerUserId != userId)
            {
                throw new ForbiddenException();
            }
        }

        if (!string.IsNullOrWhiteSpace(
            request.AssignedUserId))
        {
            if (!project.HasMember(
                request.AssignedUserId))
            {
                throw new BadRequestException(
                    "User is not project member");
            }
        }

        var workItem = new WorkItem(
            request.Title,
            request.Comment,
            userId,
            request.ProjectId,
            request.Status,
            request.Priority);

        if (!string.IsNullOrWhiteSpace(
            request.AssignedUserId))
        {
            workItem.AssignUser(
                request.AssignedUserId);
        }

        await _workItemRepository.AddAsync(
            workItem);

        return workItem.Id;
    }
}