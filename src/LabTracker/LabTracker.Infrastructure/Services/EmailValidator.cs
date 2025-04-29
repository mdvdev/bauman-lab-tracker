using System.ComponentModel.DataAnnotations;
using LabTracker.Domain.Abstractions;

namespace LabTracker.Infrastructure.Services;

public class EmailValidator : IEmailValidator
{
    public bool IsValidEmail(string email)
    {
        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(email);
    }
}