using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id);

    Task<IEnumerable<Project>> GetAllAsync();

    Task<IEnumerable<Project>> GetByManagerAsync(
        string managerId);

    Task<IEnumerable<Project>> GetByMemberAsync(
        string userId);

    Task AddAsync(Project project);

    Task UpdateAsync(Project project);

    Task DeleteAsync(Project project);

    IQueryable<Project> Query();
}