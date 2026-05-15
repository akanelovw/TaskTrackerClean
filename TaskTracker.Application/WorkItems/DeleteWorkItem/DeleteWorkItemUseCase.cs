using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.WorkItems.DeleteWorkItem;

public class DeleteWorkItemUseCase
{
    private readonly IWorkItemRepository _repo;

    public DeleteWorkItemUseCase(IWorkItemRepository repo)
    {
        _repo = repo;
    }

    public async Task Execute(DeleteWorkItemRequest request)
    {
        var item = await _repo.GetByIdAsync(request.Id);

        if (item == null)
            throw new Exception("WorkItem not found");

        _repo.Delete(item);

        await _repo.SaveChangesAsync();
    }
}