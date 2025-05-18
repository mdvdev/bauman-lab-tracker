namespace LabTracker.Shared.Contracts;

public interface IFileValidatorFactory
{
    IFileValidator GetFileValidator(string fileName);
}