namespace TaskTracker.Application.Projects.ChangeProjectStatus;

public class ChangeProjectStatusRequest
{
    public int ProjectId { get; set; }

    public string Status { get; set; } = null!;
}