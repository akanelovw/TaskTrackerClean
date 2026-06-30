using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Documents.GetDocument;

public class GetDocumentUseCase
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserService _userService;

    public GetDocumentUseCase(
        IDocumentRepository documentRepository,
        IProjectRepository projectRepository,
        IUserService userService)
    {
        _documentRepository = documentRepository;
        _projectRepository = projectRepository;
        _userService = userService;
    }

    public async Task<GetDocumentResponse> Execute(
        GetDocumentRequest request)
    {
        var document =
            await _documentRepository.GetByIdAsync(
                request.DocumentId);

        if (document == null)
            throw new NotFoundException("Document not found");

        var project =
            await _projectRepository.GetByIdAsync(
                document.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        var currentUserId =
            _userService.GetCurrentUserId();

        var isAdmin =
            _userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager);

        if (!isAdmin)
        {
            if (_userService.IsInRole(Roles.ProjectManager))
            {
                if (project.ManagerUserId != currentUserId)
                    throw new ForbiddenException();
            }
            else
            {
                if (!project.HasMember(currentUserId))
                    throw new ForbiddenException();
            }
        }

        return new GetDocumentResponse
        {
            Id = document.Id,
            FileName = document.FileName,
            FilePath = document.FilePath,
            ProjectId = document.ProjectId
        };
    }
}