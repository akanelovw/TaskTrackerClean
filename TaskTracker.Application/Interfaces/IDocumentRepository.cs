using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IDocumentRepository
{
    Task<List<Document>> GetByProjectIdAsync(int projectId);

    Task<Document?> GetByIdAsync(int id);

    Task AddAsync(Document document);

    Task DeleteAsync(Document document);
}