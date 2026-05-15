using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IWorkItemRepository
{
    Task<WorkItem?> GetByIdAsync(int id);
    Task AddAsync(WorkItem workItem);
    void Delete(WorkItem workItem);
    Task SaveChangesAsync();
}