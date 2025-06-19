namespace LabTracker.Shared.Contracts;

public interface IFileService
{
    Task<string> SaveFileAsync(Stream stream, string saveDirectory, string fileName);

    void DeleteFile(string fileName);
}