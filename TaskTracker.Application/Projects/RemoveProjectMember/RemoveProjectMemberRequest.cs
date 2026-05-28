namespace TaskTracker.Application.Projects.RemoveProjectMember;

public class RemoveProjectMemberRequest
{
    public int ProjectId { get; set; }

    public string UserId { get; set; } = null!;
}