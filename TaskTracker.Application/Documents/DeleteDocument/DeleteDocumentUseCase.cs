using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Documents.DeleteDocument;

public class DeleteDocumentUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IFileStorageService _fileStorageService;

    public DeleteDocumentUseCase(
        IProjectRepository projectRepository,
        IFileStorageService fileStorageService)
    {
        _projectRepository = projectRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task Execute(DeleteDocumentRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new Exception("Project not found");

        var document = project.Documents
            .FirstOrDefault(x => x.Id == request.DocumentId);

        if (document == null)
            throw new Exception("Document not found");

        _fileStorageService.DeleteFile(document.FilePath);

        project.RemoveDocument(document);

        await _projectRepository.SaveChangesAsync();
    }
}