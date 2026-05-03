using TaskTracker.Application.Interfaces;

public class AssignUserUseCase
{
    private readonly IWorkItemRepository _repo;

    public AssignUserUseCase(IWorkItemRepository repo)
    {
        _repo = repo;
    }

    public async Task Execute(AssignUserRequest request)
    {
        var item = await _repo.GetByIdAsync(request.WorkItemId);

        if (item == null)
            throw new Exception("Not found");

        item.AssignUser(request.UserId);

        await _repo.SaveChangesAsync();
    }
}