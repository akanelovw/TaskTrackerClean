using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Projects.ChangeProjectStatus;

public class ChangeProjectStatusRequest
{
    public int ProjectId { get; set; }

    public ProjectStatus Status { get; set; }
}