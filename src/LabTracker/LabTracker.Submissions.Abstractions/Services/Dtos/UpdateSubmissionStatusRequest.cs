using LabTracker.Submissions.Domain;

namespace LabTracker.Submissions.Abstractions.Services.Dtos;

public class UpdateSubmissionStatusRequest
{
    public SubmissionStatus SubmissionStatus { get; set; }
    public int? Score { get; set; }
    public string? Comment { get; set; }
}