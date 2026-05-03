using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.WorkItems.CreateWorkItem;

public class CreateWorkItemUseCase
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IUserService _userService;

    public CreateWorkItemUseCase(
        IWorkItemRepository workItemRepository,
        IUserService userService)
    {
        _workItemRepository = workItemRepository;
        _userService = userService;
    }

    public async Task Execute(CreateWorkItemRequest request)
    {
        var userId = _userService.GetCurrentUserId();

        var workItem = new WorkItem(
            request.Title,
            userId,
            request.ProjectId,
            request.Priority
        );

        if (!string.IsNullOrEmpty(request.AssignedUserId))
        {
            workItem.AssignUser(request.AssignedUserId);
        }

        await _workItemRepository.AddAsync(workItem);
        await _workItemRepository.SaveChangesAsync();
    }
}