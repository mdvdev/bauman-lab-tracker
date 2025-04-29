using System.Text.RegularExpressions;

namespace LabTracker.Domain.ValueObjects;

public partial record Name
{
    private static readonly Regex ValidNameRegex = MyRegex();

    public string Value { get; init; }

    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value), "Name cannot be null or empty.");
        }

        if (!IsValid(value))
        {
            throw new ArgumentException($"Name '{value}' is invalid.", nameof(value));
        }
        
        Value = value;
    }

    private bool IsValid(string value)
    {
        return ValidNameRegex.IsMatch(value);
    }

    [GeneratedRegex(@"^\p{L}+$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}