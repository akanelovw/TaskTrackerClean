using TaskTracker.Application.Interfaces;

namespace TaskTracker.Application.Projects.DeleteProject;

public class DeleteProjectUseCase
{
    private readonly IProjectRepository _repo;

    public DeleteProjectUseCase(IProjectRepository repo)
    {
        _repo = repo;
    }

    public async Task Execute(DeleteProjectRequest request)
    {
        var project = await _repo.GetByIdAsync(request.Id);

        if (project == null)
            throw new Exception("Project not found");

        _repo.Delete(project);

        await _repo.SaveChangesAsync();
    }
}