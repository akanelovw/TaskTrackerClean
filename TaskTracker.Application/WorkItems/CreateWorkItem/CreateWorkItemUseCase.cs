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

    public async Task<CreateWorkItemResponse> Execute(CreateWorkItemRequest request)
    {
        var userId =
            _userService.GetCurrentUserId();
        var project =
            await _projectRepository
                .GetByIdAsync(request.ProjectId);

        if (project == null)
        {
            throw new NotFoundException("Project not found");
        }

        if (!_userService.IsInRole(Roles.Admin))
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
            userId,
            request.ProjectId,
            request.Priority
        );

        if (!string.IsNullOrWhiteSpace(
            request.AssignedUserId))
        {
            workItem.AssignUser(
                request.AssignedUserId);
        }

        await _workItemRepository
            .AddAsync(workItem);

        return new CreateWorkItemResponse
        {
            Id = workItem.Id,
            Title = workItem.Title,
            ProjectId = workItem.ProjectId,
            AssignedUserId = workItem.AssignedUserId,
            Status = workItem.Status.ToString()
        };
    }
}