namespace TaskTracker.Application.Projects.AssignProjectManager;

public class AssignProjectManagerRequest
{
    public int ProjectId { get; set; }

    public string UserId { get; set; } = null!;
}