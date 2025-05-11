namespace LabTracker.Infrastructure.Abstractions;

public interface IFileValidatorFactory
{
    IFileValidator GetFileValidator(string fileName);
}