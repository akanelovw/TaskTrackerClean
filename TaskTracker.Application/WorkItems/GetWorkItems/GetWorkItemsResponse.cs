using TaskTracker.Application.Projects.GetWorkItems;

namespace TaskTracker.Application.WorkItems.GetWorkItems;

public class GetWorkItemsResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Comment { get; set; }

    public string? AssignedUserId { get; set; }

    public int ProjectId { get; set; }

    public string Status { get; set; } = null!;

    public string Priority { get; set; } = null!;

    public WorkItemsMemberResponse? AssignedUser { get; set; }
}