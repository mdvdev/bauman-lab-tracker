using Shared;

namespace LabTracker.Labs.Abstractions.Services.Dtos;

[NotAllNull]
public record UpdateLabRequest(
    string? Name,
    DateTimeOffset? Deadline,
    int? Score,
    int? ScoreAfterDeadline
);