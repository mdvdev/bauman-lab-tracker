namespace LabTracker.Submissions.Abstractions.Services.Dtos;

public record CreateSubmissionRequest(
    Guid StudentId,
    Guid LabId,
    Guid SlotId
);