using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.WorkItems.GetWorkItems;

public class GetWorkItemsRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }

    public WorkItemStatus? Status { get; set; }

    public int? ProjectId { get; set; }

    public string? AssignedUserId { get; set; }
}