namespace LabTracker.Application.Contracts;

public interface IFileValidator
{
    void ValidateFile(Stream stream, string fileName);
}