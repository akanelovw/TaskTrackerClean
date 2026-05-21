using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Projects.CreateProject;

public class CreateProjectUseCase
{
    private readonly IProjectRepository _repo;
    private readonly IUserService _userService;

    public CreateProjectUseCase(
        IProjectRepository repo,
        IUserService userService)
    {
        _repo = repo;
        _userService = userService;
    }

    public async Task<int> Execute(CreateProjectRequest request)
    {
        var userId = _userService.GetCurrentUserId();

        var project = new Project(
            request.Title,
            request.CustomerCompany,
            request.ExecutorCompany,
            request.StartTime,
            request.EndTime,
            request.Priority,
            userId
        );

        await _repo.AddAsync(project);
        await _repo.SaveChangesAsync();

        return project.Id;
    }
}