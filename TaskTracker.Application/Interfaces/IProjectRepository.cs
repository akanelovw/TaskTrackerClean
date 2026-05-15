using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id);

    Task<List<Project>> GetAllAsync();

    Task AddAsync(Project project);

    void Delete(Project project);

    Task SaveChangesAsync();
}