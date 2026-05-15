using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure.Persistence;

namespace TaskTracker.Infrastructure.Repositories;

public class WorkItemRepository : IWorkItemRepository
{
    private readonly ApplicationDbContext _context;

    public WorkItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkItem?> GetByIdAsync(int id)
    {
        return await _context.WorkItems
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<WorkItem>> GetByProjectIdAsync(int projectId)
    {
        return await _context.WorkItems
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task AddAsync(WorkItem workItem)
    {
        await _context.WorkItems.AddAsync(workItem);
    }

    public void Delete(WorkItem workItem)
    {
        _context.WorkItems.Remove(workItem);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}