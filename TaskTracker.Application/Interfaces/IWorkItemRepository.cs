using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IWorkItemRepository
{
    Task<WorkItem?> GetByIdAsync(int id);

    Task<List<WorkItem>> GetByProjectIdAsync(int projectId);

    Task AddAsync(WorkItem workItem);

    Task UpdateAsync(WorkItem workItem);

    Task DeleteAsync(WorkItem workItem);
    Task<List<WorkItem>> GetAllAsync();

    Task<List<WorkItem>> GetByAssigneeAsync(string userId);
}