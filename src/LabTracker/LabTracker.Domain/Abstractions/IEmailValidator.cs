namespace LabTracker.Domain.Abstractions;

public interface IEmailValidator
{
    bool IsValidEmail(string email);
}