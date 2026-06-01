using TaskTracker.Application.Common;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.WorkItems.GetWorkItems;

public class GetWorkItemsUseCase
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserService _userService;

    public GetWorkItemsUseCase(
        IWorkItemRepository workItemRepository,
        IProjectRepository projectRepository,
        IUserService userService)
    {
        _workItemRepository = workItemRepository;
        _projectRepository = projectRepository;
        _userService = userService;
    }

    public async Task<List<GetWorkItemsResponse>> Execute(
        GetWorkItemsRequest request)
    {
        var userId = _userService.GetCurrentUserId();

        var query = _workItemRepository.Query();

        var isAdmin =
            _userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager);

        if (!isAdmin)
        {
            if (_userService.IsInRole(Roles.ProjectManager))
            {
                var projects =
                    await _projectRepository
                        .GetByManagerAsync(userId);

                var projectIds =
                    projects.Select(x => x.Id).ToList();

                query = query.Where(x =>
                    projectIds.Contains(x.ProjectId));
            }
            else
            {
                query = query.Where(x =>
                    x.AssignedUserId == userId);
            }
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x =>
                x.Status == request.Status.Value);
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x =>
                x.ProjectId == request.ProjectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.AssignedUserId))
        {
            query = query.Where(x =>
                x.AssignedUserId == request.AssignedUserId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.Title.Contains(request.Search));
        }

        query = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        return query.Select(x => new GetWorkItemsResponse
        {
            Id = x.Id,
            Title = x.Title,
            AssignedUserId = x.AssignedUserId,
            ProjectId = x.ProjectId,
            Status = x.Status.ToString(),
            Priority = x.Priority.ToString()
        }).ToList();
    }
}