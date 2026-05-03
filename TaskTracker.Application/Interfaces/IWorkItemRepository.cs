using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IWorkItemRepository
{
    Task<WorkItem?> GetByIdAsync(int id);
    Task AddAsync(WorkItem workItem);
    Task DeleteAsync(WorkItem workItem);
    Task SaveChangesAsync();
}