namespace LabTracker.Submissions.Domain;

public class Submission
{
    public Guid Id { get; }
    public Guid StudentId { get; }
    public Guid LabId { get; }
    public Guid SlotId { get; }
    public Guid CourseId { get; }
    public SubmissionStatus SubmissionStatus { get; private set; }
    public string? Comment { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public double Priority { get; private set;  }

    private Submission(
        Guid id,
        Guid studentId,
        Guid labId,
        Guid slotId,
        Guid courseId,
        SubmissionStatus submissionStatus,
        string? comment,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        double priority)
    {
        if (studentId == Guid.Empty) throw new ArgumentException("StudentId cannot be empty.", nameof(studentId));
        if (labId == Guid.Empty) throw new ArgumentException("LabId cannot be empty.", nameof(labId));
        if (slotId == Guid.Empty) throw new ArgumentException("SlotId cannot be empty.", nameof(slotId));
        if (courseId == Guid.Empty) throw new ArgumentException("CourseId cannot be empty.", nameof(courseId));
        if (priority < 0) throw new ArgumentOutOfRangeException(nameof(priority), "Priority must be non-negative.");

        Id = id;
        StudentId = studentId;
        LabId = labId;
        SlotId = slotId;
        CourseId = courseId;
        SubmissionStatus = submissionStatus;
        Comment = comment;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Priority = priority;
    }

    public static Submission CreateNew(
        Guid studentId,
        Guid labId,
        Guid slotId,
        Guid courseId,
        double priority)
    {
        return new Submission(
            Guid.NewGuid(),
            studentId,
            labId,
            slotId,
            courseId,
            SubmissionStatus.Pending,
            null,
            DateTimeOffset.UtcNow,
            null,
            priority
        );
    }

    public static Submission Restore(
        Guid id,
        Guid studentId,
        Guid labId,
        Guid slotId,
        Guid courseId,
        SubmissionStatus submissionStatus,
        string? comment,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        double priority)
    {
        return new Submission(
            id,
            studentId,
            labId,
            slotId,
            courseId,
            submissionStatus,
            comment,
            createdAt,
            updatedAt,
            priority
        );
    }

    public void UpdateStatus(SubmissionStatus newSubmissionStatus, string? comment = null)
    {
        SubmissionStatus = newSubmissionStatus;
        Comment = comment;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}