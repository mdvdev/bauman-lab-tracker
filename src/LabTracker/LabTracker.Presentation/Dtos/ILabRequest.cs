namespace LabTracker.Presentation.Dtos;

public interface ILabRequest
{
    string Name { get; }
    string Description { get; }
    DateTimeOffset Deadline { get; }
    int Score { get; }
    int ScoreAfterDeadline { get; }
}