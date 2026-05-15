using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure.Persistence;

namespace TaskTracker.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects
            .Include(x => x.Documents)
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Project>> GetAllAsync()
    {
        return await _context.Projects
            .Include(x => x.Documents)
            .Include(x => x.Members)
            .ToListAsync();
    }

    public async Task<List<Project>> GetProjectsForUserAsync(string userId)
    {
        return await _context.Projects
            .Include(x => x.Documents)
            .Include(x => x.Members)
            .Where(x =>
                x.ManagerUserId == userId ||
                x.Members.Any(m => m.UserId == userId))
            .ToListAsync();
    }

    public async Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
    }

    public void Delete(Project project)
    {
        _context.Projects.Remove(project);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}