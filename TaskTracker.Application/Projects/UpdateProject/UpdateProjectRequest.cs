using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Projects.UpdateProject;

public class UpdateProjectRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CustomerCompany { get; set; }
    public string ExecutorCompany { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public ProjectPriority Priority { get; set; }
}