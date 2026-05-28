using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Documents.AddDocument;

public class AddDocumentUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUserService _userService;

    public AddDocumentUseCase(
        IProjectRepository projectRepository,
        IFileStorageService fileStorageService,
        IUserService userService)
    {
        _projectRepository = projectRepository;
        _fileStorageService = fileStorageService;
        _userService = userService;
    }

    public async Task Execute(AddDocumentRequest request)
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

        var path = await _fileStorageService.SaveFileAsync(
            request.FileStream,
            request.FileName,
            request.ProjectId);

        var document = new Document(
            request.FileName,
            path,
            request.ProjectId);

        await _projectRepository.UpdateAsync(project);
    }
}