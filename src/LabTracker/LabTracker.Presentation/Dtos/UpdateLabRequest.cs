namespace LabTracker.Presentation.Dtos;

public record UpdateLabRequest(
    string Name,
    string Description,
    DateTimeOffset Deadline,
    int Score,
    int ScoreAfterDeadline) : ILabRequest;