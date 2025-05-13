using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Dtos.Requests;

public class UpdateSubmissionStatusRequest
{
    public Status Status { get; set; }
    public int? Score { get; set; }
    public string? Comment { get; set; }
}