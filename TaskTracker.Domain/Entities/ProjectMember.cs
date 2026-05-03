namespace TaskTracker.Domain.Entities;

public class ProjectMember
{
    public int ProjectId { get; private set; }
    public string UserId { get; private set; }

    public ProjectMember(int projectId, string userId)
    {
        ProjectId = projectId;
        UserId = userId;
    }
}