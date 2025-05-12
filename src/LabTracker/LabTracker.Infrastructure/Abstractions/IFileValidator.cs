namespace LabTracker.Infrastructure.Abstractions;

public interface IFileValidator
{
    void ValidateFile(Stream stream, string fileName);
}