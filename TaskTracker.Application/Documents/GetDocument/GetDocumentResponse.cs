namespace TaskTracker.Application.Documents.GetDocument;

public class GetDocumentResponse
{
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public int ProjectId { get; set; }
}