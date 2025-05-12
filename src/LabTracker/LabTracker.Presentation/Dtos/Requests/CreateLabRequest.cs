namespace LabTracker.Presentation.Dtos.Requests;

public class CreateLabRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public int Score { get; set; }
    public int ScoreAfterDeadline { get; set; }
}