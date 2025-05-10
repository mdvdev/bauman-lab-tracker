namespace LabTracker.Application.Contracts;

public interface IFileValidatorFactory
{
    IFileValidator GetFileValidator(string fileName);
}