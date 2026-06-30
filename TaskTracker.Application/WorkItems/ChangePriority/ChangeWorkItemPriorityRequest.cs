using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.WorkItems.ChangePriority;

public class ChangeWorkItemPriorityRequest
{
    public int WorkItemId { get; set; }

    public WorkItemPriority Priority { get; set; }
}