using TaskTracker.Domain.Entities;
using TaskTracker.Application.WorkItems.GetWorkItems;

namespace TaskTracker.Application.Common.Mappings;

public static class WorkItemMapping
{
    public static GetWorkItemsResponse ToResponse(WorkItem x)
    {
        return new GetWorkItemsResponse
        {
            Id = x.Id,
            Title = x.Title,
            AssignedUserId = x.AssignedUserId,
            ProjectId = x.ProjectId,
            Status = x.Status.ToString(),
            Priority = x.Priority.ToString()
        };
    }
}