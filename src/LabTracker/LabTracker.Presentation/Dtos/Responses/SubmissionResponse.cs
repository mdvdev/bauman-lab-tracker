using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Dtos.Responses;

public class SubmissionResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid LabId { get; set; }
    public Guid SlotId { get; set; }
    public Status Status { get; set; }
    public int? Score { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public static SubmissionResponse Create(Submission submission)
    {
        return new SubmissionResponse
        {
            Id = submission.Id,
            StudentId = submission.StudentId,
            LabId = submission.LabId,
            SlotId = submission.SlotId,
            Status = submission.Status,
            Score = submission.Score,
            Comment = submission.Comment,
            CreatedAt = submission.CreatedAt,
            UpdatedAt = submission.UpdatedAt
        };
    }
}