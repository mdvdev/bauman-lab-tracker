using LabTracker.Domain.Abstractions;

namespace LabTracker.Domain.ValueObjects;

public record Email
{
    public string Value { get; init; }

    public Email(string value, IEmailValidator validator)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value), "Email cannot be null or empty.");
        }

        if (!validator.IsValidEmail(value))
        {
            throw new ArgumentException($"{Value} is not a valid email address.");
        }
        
        Value = value;
    }
}