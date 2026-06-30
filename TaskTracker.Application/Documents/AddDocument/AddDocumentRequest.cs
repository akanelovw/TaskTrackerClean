namespace TaskTracker.Application.Documents.AddDocument;

public class AddDocumentRequest
{
    public int ProjectId { get; set; }

    public string FileName { get; set; }

    public Stream FileStream { get; set; } = null!;
}