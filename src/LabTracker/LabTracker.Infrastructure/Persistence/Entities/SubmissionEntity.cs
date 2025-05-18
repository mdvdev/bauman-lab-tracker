using LabTracker.Submissions.Domain;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class SubmissionEntity
{
    public Guid Id { get; init; }
    public Guid StudentId { get; set; }
    public UserEntity Student { get; set; }
    public Guid LabId { get; set; }
    public LabEntity Lab { get; set; }
    public SubmissionStatus SubmissionStatus { get; set; }
    public int? Score { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid SlotId { get; set; }
    public SlotEntity Slot { get; set; }
    public Guid CourseId { get; set; }
    public CourseEntity Course { get; set; }

    public Submission ToDomain()
    {
        return Submission.Restore(
            id: Id,
            studentId: StudentId,
            labId: LabId,
            slotId: SlotId,
            courseId: CourseId,
            submissionStatus: SubmissionStatus,
            score: Score,
            comment: Comment,
            createdAt: CreatedAt,
            updatedAt: UpdatedAt);
    }

    public static SubmissionEntity FromDomain(Submission domain)
    {
        return new SubmissionEntity
        {
            Id = domain.Id,
            StudentId = domain.StudentId,
            LabId = domain.LabId,
            SubmissionStatus = domain.SubmissionStatus,
            Score = domain.Score,
            Comment = domain.Comment,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,
            SlotId = domain.SlotId,
            CourseId = domain.CourseId,
        };
    }
}