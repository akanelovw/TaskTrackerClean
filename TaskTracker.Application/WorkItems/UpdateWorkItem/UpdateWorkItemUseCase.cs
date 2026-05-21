using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.WorkItems.UpdateWorkItem;

public class UpdateWorkItemUseCase
{
    private readonly IWorkItemRepository _repo;

    public UpdateWorkItemUseCase(IWorkItemRepository repo)
    {
        _repo = repo;
    }

    public async Task Execute(UpdateWorkItemRequest request)
    {
        var item = await _repo.GetByIdAsync(request.Id);

        if (item == null)
            throw new Exception("Work item not found");

        item.UpdateTitle(request.Title);

        item.UpdateComment(request.Comment);

        item.ChangePriority(request.Priority);

        await _repo.SaveChangesAsync();
    }
}