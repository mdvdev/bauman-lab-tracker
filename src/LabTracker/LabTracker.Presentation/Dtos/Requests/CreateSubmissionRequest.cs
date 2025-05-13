namespace LabTracker.Presentation.Dtos.Requests;

public class CreateSubmissionRequest
{
    public Guid LabId { get; set; }
    public Guid SlotId { get; set; }
}