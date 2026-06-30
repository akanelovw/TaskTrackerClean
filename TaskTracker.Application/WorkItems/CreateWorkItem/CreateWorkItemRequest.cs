using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.WorkItems.CreateWorkItem;

public class CreateWorkItemRequest
{
    public string Title { get; set; }

    public int ProjectId { get; set; }

    public string? Comment { get; set; }

    public WorkItemStatus Status { get; set; }

    public WorkItemPriority Priority { get; set; }

    public string? AssignedUserId { get; set; }
}