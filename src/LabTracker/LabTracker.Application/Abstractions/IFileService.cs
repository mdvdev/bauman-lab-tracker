namespace LabTracker.Application.Abstractions;

public interface IFileService
{
    Task<string> SaveFileAsync(Stream stream, string saveDirectory, string fileName);

    void DeleteFile(string fileName);
}