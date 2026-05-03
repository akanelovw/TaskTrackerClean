using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.WorkItems.ChangeStatus;

public class ChangeWorkItemStatusUseCase
{
    private readonly IWorkItemRepository _workItemRepository;

    public ChangeWorkItemStatusUseCase(IWorkItemRepository workItemRepository)
    {
        _workItemRepository = workItemRepository;
    }

    public async Task Execute(ChangeWorkItemStatusRequest request)
    {
        var workItem = await _workItemRepository.GetByIdAsync(request.WorkItemId);

        if (workItem == null)
            throw new Exception("WorkItem not found");

        workItem.ChangeStatus(request.Status);

        await _workItemRepository.SaveChangesAsync();
    }
}