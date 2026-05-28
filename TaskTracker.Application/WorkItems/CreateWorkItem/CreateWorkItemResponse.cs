namespace TaskTracker.Application.WorkItems.CreateWorkItem;

public class CreateWorkItemResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int ProjectId { get; set; }

    public string? AssignedUserId { get; set; }

    public string Status { get; set; } = null!;
}