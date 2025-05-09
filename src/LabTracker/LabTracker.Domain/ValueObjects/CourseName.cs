using System.Text.RegularExpressions;

namespace LabTracker.Domain.ValueObjects;

public partial record CourseName
{
    private static readonly Regex ValidCourseNameRegex = CourseNameRegex();

    public string Value { get; init; }

    public CourseName(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value), "Course name cannot be null or empty.");

        if (!IsValid(value))
        {
            throw new ArgumentException($"Course name '{value}' is invalid.", nameof(value));
        }

        Value = value;
    }

    private static bool IsValid(string value)
    {
        return ValidCourseNameRegex.IsMatch(value);
    }

    [GeneratedRegex(@"^[\p{L} ]+$", RegexOptions.Compiled)]
    private static partial Regex CourseNameRegex();
}