using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Projects.GetProjectDetails;

public class GetProjectDetailsUseCase
{
    private readonly IProjectRepository _repo;
    private readonly IUserService _userService;

    public GetProjectDetailsUseCase(
        IProjectRepository repo,
        IUserService userService)
    {
        _repo = repo;
        _userService = userService;
    }

    public async Task<GetProjectDetailsResponse> Execute(int id)
    {
        var project = await _repo.GetByIdAsync(id);

        if (project == null)
            throw new NotFoundException("Project not found");

        var userId =
            _userService.GetCurrentUserId();

        if (_userService.IsInRole(Roles.Admin))
            return Map(project);

        if (_userService.IsInRole(Roles.ChiefProjectManager))
            return Map(project);

        if (_userService.IsInRole(Roles.ProjectManager))
        {
            if (project.ManagerUserId != userId)
                throw new ForbiddenException();

            return Map(project);
        }

        if (!project.HasMember(userId))
            throw new ForbiddenException();

        return Map(project);
    }

    private static GetProjectDetailsResponse Map(
        Domain.Entities.Project project)
    {
        return new GetProjectDetailsResponse
        {
            Id = project.Id,
            Title = project.Title,
            CustomerCompany = project.CustomerCompany,
            ExecutorCompany = project.ExecutorCompany,
            StartTime = project.StartTime,
            EndTime = project.EndTime,
            Priority = project.Priority.ToString(),
            Status = project.Status.ToString(),
            ManagerUserId = project.ManagerUserId
        };
    }
}