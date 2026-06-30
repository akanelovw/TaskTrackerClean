using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Common.Mappings;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Documents.GetProjectDocuments;

namespace TaskTracker.Application.Projects.GetProjectDetails;

public class GetProjectDetailsUseCase
{
    private readonly IProjectRepository _repo;
    private readonly IUserService _userService;
    private readonly IUserManagementService _users;
    private readonly IDocumentRepository _documents;

    public GetProjectDetailsUseCase(
        IProjectRepository repo,
        IUserService userService,
        IUserManagementService users,
        IDocumentRepository documents)
    {
        _repo = repo;
        _userService = userService;
        _users = users;
        _documents = documents;
    }

    public async Task<GetProjectDetailsResponse> Execute(int id)
    {
        var project = await _repo.GetByIdAsync(id);

        if (project == null)
            throw new NotFoundException("Project not found");

        var userId = _userService.GetCurrentUserId();

        var allowed =
            _userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager) ||
            project.HasMember(userId) ||
            project.ManagerUserId == userId;

        if (!allowed)
            throw new ForbiddenException();

        var membersList = new List<ProjectMemberResponse>();

        foreach (var member in project.Members)
        {
            var user = await _users.GetByIdAsync(member.UserId);

            if (user == null)
                continue;

            membersList.Add(new ProjectMemberResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Role = user.Role
            });
        }

        string? managerName = null;

        if (!string.IsNullOrEmpty(project.ManagerUserId))
        {
            var user = await _users.GetByIdAsync(project.ManagerUserId);
            managerName = user?.FullName;
        }

        var documentsDomain = await _documents.GetByProjectIdAsync(id);

        var documents = documentsDomain
            .Select(DocumentMapping.ToResponse)
            .ToList();

        return ProjectMapping.ToDetails(
            project,
            managerName,
            membersList,
            documents);
    }
}