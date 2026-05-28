namespace TaskTracker.Application.WorkItems.GetWorkItems;

public class GetWorkItemsRequest
{
    public int? ProjectId { get; set; }

    public string? AssignedUserId { get; set; }

    public string? Status { get; set; }
}