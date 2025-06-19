using System.Text.RegularExpressions;

namespace LabTracker.Labs.Domain;

public class Lab
{
    public Guid Id { get; }
    public Guid CourseId { get; }
    public string Name { get; private set; }
    public string? DescriptionUri { get; private set; }
    public DateTimeOffset Deadline { get; private set; }
    public int Score { get; private set; }
    public int ScoreAfterDeadline { get; private set; }

    private static readonly Regex ValidNameRegex = new(@"^[\p{L}0-9 _-]+$", RegexOptions.Compiled);

    private Lab(
        Guid id,
        Guid courseId,
        string name,
        string? descriptionUri,
        DateTimeOffset deadline,
        int score,
        int scoreAfterDeadline)
    {
        if (string.IsNullOrWhiteSpace(name) || !IsValidName(name))
            throw new ArgumentException($"Lab name '{name}' is invalid.", nameof(name));

        if (score < 0)
            throw new ArgumentOutOfRangeException(nameof(score), "Score must be non-negative.");

        if (scoreAfterDeadline < 0)
            throw new ArgumentOutOfRangeException(nameof(scoreAfterDeadline),
                "ScoreAfterDeadline must be non-negative.");

        Id = id;
        CourseId = courseId;
        Name = name;
        DescriptionUri = descriptionUri;
        Deadline = deadline;
        Score = score;
        ScoreAfterDeadline = scoreAfterDeadline;
    }

    public static Lab CreateNew(
        Guid courseId,
        string name,
        string? descriptionUri,
        DateTimeOffset deadline,
        int score,
        int scoreAfterDeadline)
    {
        return new Lab(
            Guid.NewGuid(),
            courseId,
            name,
            descriptionUri,
            deadline,
            score,
            scoreAfterDeadline);
    }

    public static Lab Restore(
        Guid id,
        Guid courseId,
        string name,
        string descriptionUri,
        DateTimeOffset deadline,
        int score,
        int scoreAfterDeadline)
    {
        return new Lab(id, courseId, name, descriptionUri, deadline, score, scoreAfterDeadline);
    }

    public void Update(
        string? name = null,
        string? descriptionUri = null,
        DateTimeOffset? deadline = null,
        int? score = null,
        int? scoreAfterDeadline = null)
    {
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name) || !IsValidName(name))
                throw new ArgumentException($"Lab name '{name}' is invalid.", nameof(name));
            Name = name;
        }

        if (descriptionUri is not null)
        {
            DescriptionUri = descriptionUri;
        }

        if (deadline is not null)
            Deadline = deadline.Value;

        if (score is not null)
        {
            if (score < 0)
                throw new ArgumentOutOfRangeException(nameof(score), "Score must be non-negative.");
            Score = score.Value;
        }

        if (scoreAfterDeadline is not null)
        {
            if (scoreAfterDeadline < 0)
                throw new ArgumentOutOfRangeException(nameof(scoreAfterDeadline),
                    "ScoreAfterDeadline must be non-negative.");
            ScoreAfterDeadline = scoreAfterDeadline.Value;
        }
    }

    private static bool IsValidName(string value) => ValidNameRegex.IsMatch(value);
}