using TaskTracker.Application.Projects.GetWorkItems;
using TaskTracker.Application.WorkItems.GetWorkItems;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Mappings;

public static class WorkItemMapping
{
    public static GetWorkItemsResponse ToResponse(
        WorkItem x,
        WorkItemsMemberResponse? assignedUser)
    {
        return new GetWorkItemsResponse
        {
            Id = x.Id,
            Title = x.Title,
            Comment = x.Comment,
            AssignedUserId = x.AssignedUserId,
            ProjectId = x.ProjectId,
            Status = x.Status.ToString(),
            Priority = x.Priority.ToString(),
            AssignedUser = assignedUser
        };
    }
}