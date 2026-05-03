using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Projects.UpdateProject;

public class UpdateProjectUseCase
{
    private readonly IProjectRepository _repo;

    public UpdateProjectUseCase(IProjectRepository repo)
    {
        _repo = repo;
    }

    public async Task Execute(UpdateProjectRequest request)
    {
        var project = await _repo.GetByIdAsync(request.Id);

        if (project == null)
            throw new Exception("Project not found");

        project.Update(
            request.Title,
            request.CustomerCompany,
            request.ExecutorCompany,
            request.StartTime,
            request.EndTime,
            request.Priority
        );

        await _repo.SaveChangesAsync();
    }
}