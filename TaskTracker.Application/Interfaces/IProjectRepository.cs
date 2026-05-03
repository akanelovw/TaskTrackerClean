using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id);

    Task<List<Project>> GetAllAsync();

    Task AddAsync(Project project);

    Task DeleteAsync(Project project);

    Task SaveChangesAsync();
}