using LabTracker.Domain.Entities;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class LabEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseId { get; set; }
    public CourseEntity Course { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public int Score { get; set; }
    public int ScoreAfterDeadline { get; set; }

    public Lab ToDomain()
    {
        return new Lab
        {
            Id = Id,
            CourseId = CourseId,
            Course = Course?.ToDomain(), // Используем метод ToDomain из CourseEntity
            Name = Name,
            Description = Description,
            Deadline = Deadline,
            Score = Score,
            ScoreAfterDeadline = ScoreAfterDeadline
        };
    }

    public static LabEntity FromDomain(Lab domain)
    {
        if (domain == null)
            return null;

        return new LabEntity
        {
            Id = domain.Id,
            CourseId = domain.CourseId,
            Course = domain.Course != null ? CourseEntity.FromDomain(domain.Course) : null,
            Name = domain.Name,
            Description = domain.Description,
            Deadline = domain.Deadline,
            Score = domain.Score,
            ScoreAfterDeadline = domain.ScoreAfterDeadline
        };
    }
}