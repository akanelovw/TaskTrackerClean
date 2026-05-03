using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.WorkItems.UpdateWorkItem;

public class UpdateWorkItemRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Comment { get; set; }
    public WorkItemPriority Priority { get; set; }
}