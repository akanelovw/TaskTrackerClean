using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Mappings;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Projects.GetProjectsList;

public class GetProjectsListUseCase
{
    private readonly IProjectRepository _repository;
    private readonly IUserService _userService;

    public GetProjectsListUseCase(
        IProjectRepository repository,
        IUserService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    public async Task<List<GetProjectsListResponse>> Execute(
        GetProjectsListRequest request)
    {
        var userId = _userService.GetCurrentUserId();

        var query = _repository.Query();

        var isAdmin =
            _userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager);

        if (!isAdmin)
        {
            if (_userService.IsInRole(Roles.ProjectManager))
            {
                query = query.Where(x =>
                    x.ManagerUserId == userId);
            }
            else
            {
                query = query.Where(x =>
                    x.Members.Any(m => m.UserId == userId));
            }
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x =>
                x.Status == request.Status.Value);
        }

        if (request.Priority.HasValue)
        {
            query = query.Where(x =>
                x.Priority == request.Priority.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.Title.Contains(request.Search));
        }

        query = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        return query
            .Select(ProjectMapping.ToList)
            .ToList();
    }
}