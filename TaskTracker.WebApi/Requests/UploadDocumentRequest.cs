namespace TaskTracker.WebApi.Requests;

public class UploadDocumentRequest
{

    public IFormFile File { get; set; } = null!;
}