using TaskTracker.Domain.Entities;
using TaskTracker.Application.Documents.GetProjectDocuments;

namespace TaskTracker.Application.Common.Mappings;

public static class DocumentMapping
{
    public static GetProjectDocumentsResponse ToResponse(Document x)
    {
        return new GetProjectDocumentsResponse
        {
            Id = x.Id,
            FileName = x.FileName,
            FilePath = x.FilePath
        };
    }
}