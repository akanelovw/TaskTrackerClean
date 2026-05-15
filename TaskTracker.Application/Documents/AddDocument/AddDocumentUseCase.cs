using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Documents.AddDocument;

public class AddDocumentUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IFileStorageService _fileStorageService;

    public AddDocumentUseCase(
        IProjectRepository projectRepository,
        IFileStorageService fileStorageService)
    {
        _projectRepository = projectRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task Execute(AddDocumentRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new Exception("Project not found");

        var path = await _fileStorageService.SaveFileAsync(
            request.FileStream,
            request.FileName,
            request.ProjectId);

        var document = new Document(
            request.FileName,
            path,
            request.ProjectId);

        project.AddDocument(document);

        await _projectRepository.SaveChangesAsync();
    }
}