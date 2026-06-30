using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure.Persistence;

namespace TaskTracker.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Document>> GetByProjectIdAsync(int projectId)
    {
        return await _context.Documents
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<Document?> GetByIdAsync(int id)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Document document)
    {
        await _context.Documents.AddAsync(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Document document)
    {
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
    }
}