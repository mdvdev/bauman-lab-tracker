namespace LabTracker.Labs.Abstractions.Services.Dtos;

public record CreateLabRequest(
    string Name,
    DateTimeOffset Deadline,
    int Score,
    int ScoreAfterDeadline
);