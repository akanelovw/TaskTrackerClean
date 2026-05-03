using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class WorkItem
{
    public int Id { get; private set; }

    public string Title { get; private set; }

    public string? Comment { get; private set; }

    public string CreatedByUserId { get; private set; }

    public string? AssignedUserId { get; private set; }

    public WorkItemStatus Status { get; private set; }

    public WorkItemPriority Priority { get; private set; }

    public int ProjectId { get; private set; }

    private WorkItem() { } // EF Core

    public WorkItem(
        string title,
        string createdByUserId,
        int projectId,
        WorkItemPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required");

        if (string.IsNullOrWhiteSpace(createdByUserId))
            throw new ArgumentException("CreatedByUserId is required");

        if (projectId <= 0)
            throw new ArgumentException("ProjectId is invalid");

        Title = title;
        CreatedByUserId = createdByUserId;
        ProjectId = projectId;
        Priority = priority;
        Status = WorkItemStatus.ToDo;
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required");

        Title = title;
    }

    public void UpdateComment(string? comment)
    {
        Comment = comment;
    }

    public void ChangeStatus(WorkItemStatus status)
    {
        if (Status == WorkItemStatus.Done && status == WorkItemStatus.ToDo)
            throw new InvalidOperationException("Cannot move Done back to ToDo");

        Status = status;
    }

    public void ChangePriority(WorkItemPriority priority)
    {
        Priority = priority;
    }

    public void AssignUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required");

        AssignedUserId = userId;
    }

    public void UnassignUser()
    {
        AssignedUserId = null;
    }

    public bool IsAssigned => AssignedUserId != null;
    public bool IsDone => Status == WorkItemStatus.Done;
}