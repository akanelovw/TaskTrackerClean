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

    public async Task<List<GetWorkItemsResponse>> Execute()
    {
        var userId = _userService.GetCurrentUserId();

        var isAdmin =
            _userService.IsInRole(Roles.Admin) ||
            _userService.IsInRole(Roles.ChiefProjectManager);

        var result = new List<GetWorkItemsResponse>();

        if (isAdmin)
        {
            var all = await _workItemRepository.GetAllAsync();

            return all.Select(x => new GetWorkItemsResponse
            {
                Id = x.Id,
                Title = x.Title,
                AssignedUserId = x.AssignedUserId,
                ProjectId = x.ProjectId,
                Status = x.Status.ToString()
            }).ToList();
        }

        if (_userService.IsInRole(Roles.ProjectManager))
        {
            var projects =
                await _projectRepository.GetByManagerAsync(userId);

            var projectIds = projects.Select(p => p.Id).ToList();

            var allTasks = new List<GetWorkItemsResponse>();

            foreach (var projectId in projectIds)
            {
                var items =
                    await _workItemRepository.GetByProjectIdAsync(projectId);

                allTasks.AddRange(items.Select(x => new GetWorkItemsResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    AssignedUserId = x.AssignedUserId,
                    ProjectId = x.ProjectId,
                    Status = x.Status.ToString()
                }));
            }

            return allTasks;
        }

        var myTasks =
            await _workItemRepository.GetByAssigneeAsync(userId);

        return myTasks.Select(x => new GetWorkItemsResponse
        {
            Id = x.Id,
            Title = x.Title,
            AssignedUserId = x.AssignedUserId,
            ProjectId = x.ProjectId,
            Status = x.Status.ToString()
        }).ToList();
    }
}