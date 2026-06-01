using Microsoft.EntityFrameworkCore;

using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure.Persistence;

namespace TaskTracker.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects
            .Include(x => x.Members)
            .Include(x => x.Documents)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _context.Projects
            .Include(x => x.Members)
            .Include(x => x.Documents)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>>
        GetByManagerAsync(string managerId)
    {
        return await _context.Projects
            .Include(x => x.Members)
            .Include(x => x.Documents)
            .Where(x => x.ManagerUserId == managerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>>
        GetByMemberAsync(string userId)
    {
        return await _context.Projects
            .Include(x => x.Members)
            .Include(x => x.Documents)
            .Where(x =>
                x.Members.Any(m => m.UserId == userId))
            .ToListAsync();
    }

    public async Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();
    }

    public IQueryable<Project> Query()
    {
        return _context.Projects;
    }
}