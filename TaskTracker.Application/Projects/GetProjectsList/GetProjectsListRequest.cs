using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Projects.GetProjectsList;

public class GetProjectsListRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }

    public ProjectStatus? Status { get; set; }

    public ProjectPriority? Priority { get; set; }
}