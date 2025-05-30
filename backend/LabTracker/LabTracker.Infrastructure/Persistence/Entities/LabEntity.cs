using LabTracker.Labs.Domain;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class LabEntity
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public CourseEntity Course { get; set; }
    public string Name { get; set; }
    public string? DescriptionUri { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public int Score { get; set; }
    public int ScoreAfterDeadline { get; set; }

    public Lab ToDomain()
    {
        return Lab.Restore(
            id: Id,
            courseId: CourseId,
            name: Name,
            descriptionUri: DescriptionUri,
            deadline: Deadline,
            score: Score,
            scoreAfterDeadline: ScoreAfterDeadline);
    }

    public static LabEntity FromDomain(Lab domain)
    {
        return new LabEntity
        {
            Id = domain.Id,
            CourseId = domain.CourseId,
            Name = domain.Name,
            DescriptionUri = domain.DescriptionUri,
            Deadline = domain.Deadline,
            Score = domain.Score,
            ScoreAfterDeadline = domain.ScoreAfterDeadline
        };
    }
}