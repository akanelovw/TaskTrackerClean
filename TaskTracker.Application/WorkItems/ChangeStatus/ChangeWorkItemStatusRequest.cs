using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.WorkItems.ChangeStatus;

public class ChangeWorkItemStatusRequest
{
    public int WorkItemId { get; set; }

    public WorkItemStatus Status { get; set; }
}