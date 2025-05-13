using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class SubmissionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public UserEntity Student { get; set; }
    public Guid LabId { get; set; }
    public LabEntity Lab { get; set; }
    public string Status { get; set; }
    public int? Score { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid SlotId { get; set; }
    public SlotEntity Slot { get; set; }
    public Guid CourseId { get; set; }
    public CourseEntity Course { get; set; }

    public static Submission ToDomain(SubmissionEntity entity, IEnumerable<string> userRoles = null)
    {
        if (entity is null)
            return null;

        return new Submission
        {
            Id = entity.Id,
            StudentId = entity.StudentId,
            Student = entity.Student?.ToDomain(userRoles ?? Enumerable.Empty<string>()),
            LabId = entity.LabId,
            Lab = entity.Lab?.ToDomain(),
            Status = Enum.Parse<Status>(entity.Status),
            Score = entity.Score,
            Comment = entity.Comment,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            SlotId = entity.SlotId,
            Slot = entity.Slot?.ToDomain(),
            CourseId = entity.CourseId,
            Course = entity.Course?.ToDomain()
        };
    }

    public static SubmissionEntity FromDomain(Submission domain)
    {
        if (domain is null)
            return null;

        return new SubmissionEntity
        {
            Id = domain.Id,
            StudentId = domain.StudentId,
            Student = domain.Student != null ? UserEntity.FromDomain(domain.Student) : null,
            LabId = domain.LabId,
            Lab = domain.Lab != null ? LabEntity.FromDomain(domain.Lab) : null,
            Status = domain.Status.ToString(),
            Score = domain.Score,
            Comment = domain.Comment,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,
            SlotId = domain.SlotId,
            Slot = domain.Slot != null ? SlotEntity.FromDomain(domain.Slot) : null,
            CourseId = domain.CourseId,
            Course = domain.Course != null ? CourseEntity.FromDomain(domain.Course) : null
        };
    }
}