using TaskTracker.Application.Projects.GetProjectsList;
using TaskTracker.Application.Projects.GetProjectDetails;
using TaskTracker.Domain.Entities;
using TaskTracker.Application.Documents.GetProjectDocuments;

public static class ProjectMapping
{
    public static GetProjectsListResponse ToList(Project x)
    {
        return new GetProjectsListResponse
        {
            Id = x.Id,
            Title = x.Title,
            CustomerCompany = x.CustomerCompany,
            ExecutorCompany = x.ExecutorCompany,
            Status = x.Status.ToString(),
            Priority = x.Priority.ToString()
        };
    }

    public static GetProjectDetailsResponse ToDetails(
        Project project,
        string? managerName,
        List<ProjectMemberResponse> members,
        List<GetProjectDocumentsResponse> documents)
    {
        return new GetProjectDetailsResponse
        {
            Id = project.Id,
            Title = project.Title,
            CustomerCompany = project.CustomerCompany,
            ExecutorCompany = project.ExecutorCompany,
            StartTime = project.StartTime,
            EndTime = project.EndTime,
            Priority = project.Priority.ToString(),
            Status = project.Status.ToString(),
            ManagerUserId = project.ManagerUserId,
            ManagerName = managerName,
            Members = members,
            Documents = documents
        };
    }
}