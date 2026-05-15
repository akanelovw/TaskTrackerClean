namespace TaskTracker.Domain.Entities;

public class Document
{
    public int Id { get; private set; }

    public string FileName { get; private set; }

    public string FilePath { get; private set; }

    public int ProjectId { get; private set; }

    private Document() { }

    public Document(
        string fileName,
        string filePath,
        int projectId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("FileName is required");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("FilePath is required");

        if (projectId <= 0)
            throw new ArgumentException("Invalid ProjectId");

        FileName = fileName;
        FilePath = filePath;
        ProjectId = projectId;
    }

    public void Rename(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("FileName is required");

        FileName = fileName;
    }

    public void ChangePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("FilePath is required");

        FilePath = filePath;
    }
}