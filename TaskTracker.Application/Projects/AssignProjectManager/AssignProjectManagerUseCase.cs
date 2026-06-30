using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Projects.AssignProjectManager;

public class AssignProjectManagerUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserManagementService _userManagementService;
    private readonly IUserRoleService _roleService;
    private readonly IUserService _userService;

    public AssignProjectManagerUseCase(
        IProjectRepository projectRepository,
        IUserManagementService userManagementService,
        IUserRoleService roleService,
        IUserService userService)
    {
        _projectRepository = projectRepository;
        _userManagementService = userManagementService;
        _roleService = roleService;
        _userService = userService;
    }

    public async Task Execute(AssignProjectManagerRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new NotFoundException("Project not found");

        var currentUserId = _userService.GetCurrentUserId();

        if (!await _roleService.IsInRoleAsync(currentUserId, Roles.Admin) &&
            !await _roleService.IsInRoleAsync(currentUserId, Roles.ChiefProjectManager))
        {
            throw new ForbiddenException();
        }

        var user = await _userManagementService.GetByIdAsync(request.UserId);

        if (user == null)
            throw new NotFoundException("User not found");

        if (!await _roleService.IsInRoleAsync(request.UserId, Roles.ProjectManager) &&
            !await _roleService.IsInRoleAsync(request.UserId, Roles.ChiefProjectManager) &&
            !await _roleService.IsInRoleAsync(request.UserId, Roles.Admin))
        {
            throw new BadRequestException("User cannot be project manager");
        }


        if (request.UserId == project.ManagerUserId)
        {
            return;
        }
        project.ChangeManager(request.UserId);

        await _projectRepository.UpdateAsync(project);
    }
}