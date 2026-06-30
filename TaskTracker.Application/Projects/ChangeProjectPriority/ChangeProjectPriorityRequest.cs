using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Projects.ChangeProjectPriority;

public class ChangeProjectPriorityRequest
{
    public int ProjectId { get; set; }

    public ProjectPriority Priority { get; set; }
}