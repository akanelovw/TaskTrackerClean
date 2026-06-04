using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Common.Mappings;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Documents.GetProjectDocuments;

public class GetProjectDocumentsUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserService _userService;

    public GetProjectDocumentsUseCase(
        IProjectRepository projectRepository,
        IUserService userService)
    {
        _projectRepository = projectRepository;
        _userService = userService;
    }

    public async Task<List<GetProjectDocumentsResponse>> Execute(
        GetProjectDocumentsRequest request)
    {
        var project = await _projectRepository
            .GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        var currentUserId = _userService.GetCurrentUserId();

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

        return project.Documents
            .Select(DocumentMapping.ToResponse)
            .ToList();
    }
}