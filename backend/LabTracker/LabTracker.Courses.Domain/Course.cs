using System.Text.RegularExpressions;

namespace LabTracker.Courses.Domain;

public class Course
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public QueueMode QueueMode { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public string? PhotoUri { get; private set; }

    private static readonly Regex ValidCourseNameRegex = new(@"^[\p{L} ]+$", RegexOptions.Compiled);

    private Course(
        Guid id,
        string name,
        string description,
        QueueMode queueMode,
        DateTimeOffset createdAt,
        string? photoUri)
    {
        if (string.IsNullOrWhiteSpace(name) || !IsValidName(name))
            throw new ArgumentException($"Course name '{name}' is invalid.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Course description cannot be empty.", nameof(description));

        Id = id;
        Name = name;
        Description = description;
        QueueMode = queueMode;
        CreatedAt = createdAt;
        PhotoUri = photoUri;
    }

    public static Course CreateNew(
        string name,
        string description,
        QueueMode queueMode,
        string? photoUri = null)
    {
        return new Course(
            Guid.NewGuid(),
            name,
            description,
            queueMode,
            DateTimeOffset.UtcNow,
            photoUri);
    }

    public static Course Restore(
        Guid id,
        string name,
        string description,
        QueueMode queueMode,
        DateTimeOffset createdAt,
        string? photoUri = null)
    {
        return new Course(
            id,
            name,
            description,
            queueMode,
            createdAt,
            photoUri);
    }

    public void Update(
        string? name = null,
        string? description = null,
        QueueMode? queueMode = null,
        string? photoUri = null)
    {
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name) || !IsValidName(name))
                throw new ArgumentException($"Course name '{name}' is invalid.", nameof(name));
            Name = name;
        }

        if (description is not null)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Course description cannot be empty.", nameof(description));
            Description = description;
        }

        if (queueMode is not null)
        {
            QueueMode = queueMode.Value;
        }

        if (photoUri is not null)
        {
            PhotoUri = photoUri;
        }
    }

    private static bool IsValidName(string value)
    {
        return ValidCourseNameRegex.IsMatch(value);
    }
}