namespace TaskTracker.Application.Projects.GetProjectsList;

public class GetProjectsListResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string CustomerCompany { get; set; } = null!;

    public string ExecutorCompany { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Priority { get; set; } = null!;
}