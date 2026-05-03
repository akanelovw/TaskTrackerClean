namespace TaskTracker.Domain.Entities;

public class Document
{
    public int Id { get; private set; }

    public string FileName { get; private set; }

    public string FileLocation { get; private set; }

    public int ProjectId { get; private set; }

    public Document(string fileName, string fileLocation, int projectId)
    {
        FileName = fileName;
        FileLocation = fileLocation;
        ProjectId = projectId;
    }
}