using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Projects.GetProjectsList;

public class GetProjectsListUseCase
{
    private readonly IProjectRepository _repo;
    private readonly IUserService _userService;

    public GetProjectsListUseCase(
        IProjectRepository repo,
        IUserService userService)
    {
        _repo = repo;
        _userService = userService;
    }

    public async Task<List<Project>> Execute()
    {
        var userId = _userService.GetCurrentUserId();

        var projects = await _repo.GetAllAsync();

        return projects
            .Where(p =>
                p.ManagerUserId == userId ||
                p.Members.Any(m => m.UserId == userId))
            .ToList();
    }
}