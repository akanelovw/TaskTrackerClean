using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Documents.GetProjectDocuments;

public class GetProjectDocumentsUseCase
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectDocumentsUseCase(
        IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<List<GetProjectDocumentsResponse>> Execute(
        GetProjectDocumentsRequest request)
    {
        var project = await _projectRepository
            .GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new Exception("Project not found");

        return project.Documents
            .Select(x => new GetProjectDocumentsResponse
            {
                Id = x.Id,
                FileName = x.FileName,
                FilePath = x.FilePath
            })
            .ToList();
    }
}