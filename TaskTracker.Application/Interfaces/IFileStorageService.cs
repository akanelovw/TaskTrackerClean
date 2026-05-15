namespace TaskTracker.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(
        Stream stream,
        string fileName,
        int projectId);

    void DeleteFile(string filePath);
}