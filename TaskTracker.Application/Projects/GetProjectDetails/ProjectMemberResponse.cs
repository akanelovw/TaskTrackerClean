namespace TaskTracker.Application.Projects.GetProjectDetails;

public class ProjectMemberResponse
{
    public string UserId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Role { get; set; } = null!;
}