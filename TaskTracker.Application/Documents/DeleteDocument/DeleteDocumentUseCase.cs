using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Documents.DeleteDocument;

public class DeleteDocumentUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUserService _userService;

    public DeleteDocumentUseCase(
        IProjectRepository projectRepository,
        IFileStorageService fileStorageService,
        IUserService userService)
    {
        _projectRepository = projectRepository;
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

        var document = project.Documents
            .FirstOrDefault(x => x.Id == request.DocumentId);

        if (document == null)
            throw new NotFoundException("Document not found");

        _fileStorageService.DeleteFile(document.FilePath);

        project.RemoveDocument(document);

        await _projectRepository.UpdateAsync(project);
    }
}