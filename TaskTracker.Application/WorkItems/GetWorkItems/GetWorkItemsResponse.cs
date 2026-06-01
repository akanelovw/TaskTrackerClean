namespace TaskTracker.Application.WorkItems.GetWorkItems;

public class GetWorkItemsResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? AssignedUserId { get; set; }

    public int ProjectId { get; set; }

    public string Status { get; set; } = null!;

    public string Priority { get; set; } = null!;
}