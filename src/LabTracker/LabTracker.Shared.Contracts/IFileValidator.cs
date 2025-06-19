namespace LabTracker.Shared.Contracts;

public interface IFileValidator
{
    void ValidateFile(Stream stream, string fileName);
}