using TaskTracker.Application.Interfaces;

namespace TaskTracker.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    public async Task<string> SaveFileAsync(
        Stream stream,
        string fileName,
        int projectId)
    {
        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "uploads",
            projectId.ToString());

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var filePath = Path.Combine(
            uploadsFolder,
            fileName);

        using var fileStream = new FileStream(
            filePath,
            FileMode.Create);

        await stream.CopyToAsync(fileStream);

        return $"/uploads/{projectId}/{fileName}";
    }

    public void DeleteFile(string filePath)
    {
        var fullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            filePath.TrimStart('/'));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}