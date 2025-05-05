namespace LabTracker.Presentation.Dtos;

public record CreateLabRequest(
    string Name,
    string Description,
    DateTimeOffset Deadline,
    int Score,
    int ScoreAfterDeadline) : ILabRequest;