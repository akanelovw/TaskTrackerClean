using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Projects.GetProjectDetails;

public class GetProjectDetailsUseCase
{
    private readonly IProjectRepository _repo;

    public GetProjectDetailsUseCase(IProjectRepository repo)
    {
        _repo = repo;
    }

    public async Task<Project> Execute(int id)
    {
        var project = await _repo.GetByIdAsync(id);

        if (project == null)
            throw new Exception("Project not found");

        return project;
    }
}