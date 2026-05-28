using TaskTracker.Application.Common;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

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

    public async Task<IEnumerable<Project>> Execute()
    {
        var userId =
            _userService.GetCurrentUserId();

        if (_userService.IsInRole(Roles.Admin))
        {
            return await _repository.GetAllAsync();
        }

        if (_userService.IsInRole(
            Roles.ChiefProjectManager))
        {
            return await _repository.GetAllAsync();
        }

        if (_userService.IsInRole(
            Roles.ProjectManager))
        {
            return await _repository
                .GetByManagerAsync(userId);
        }

        return await _repository
            .GetByMemberAsync(userId);
    }
}