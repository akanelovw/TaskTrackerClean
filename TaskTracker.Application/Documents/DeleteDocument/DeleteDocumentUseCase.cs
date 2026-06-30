using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Documents.DeleteDocument;

public class DeleteDocumentUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUserService _userService;

    public DeleteDocumentUseCase(
        IProjectRepository projectRepository,
        IDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        IUserService userService)
    {
        _projectRepository = projectRepository;
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _userService = userService;
    }

    public async Task Execute(DeleteDocumentRequest request)
    {
        var project =
            await _projectRepository.GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        var currentUserId =
            _userService.GetCurrentUserId();

        var isAdmin =
            _userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager);

        if (!isAdmin)
        {
            if (!_userService.IsInRole(Roles.ProjectManager))
                throw new ForbiddenException();

            if (project.ManagerUserId != currentUserId)
                throw new ForbiddenException();
        }

        var document =
            await _documentRepository.GetByIdAsync(request.DocumentId);

        if (document == null)
            throw new NotFoundException("Document not found");

        _fileStorageService.DeleteFile(document.FilePath);

        await _documentRepository.DeleteAsync(document);
    }
}