using LabTracker.Labs.Domain;

namespace Labs.Web.Dtos;

public record LabResponse(
    Guid Id,
    Guid CourseId,
    string Name,
    string? DescriptionUri,
    DateTimeOffset Deadline,
    int Score,
    int ScoreAfterDeadline
)
{
    public static LabResponse Create(Lab lab) => new(
        lab.Id,
        lab.CourseId,
        lab.Name,
        lab.DescriptionUri,
        lab.Deadline,
        lab.Score,
        lab.ScoreAfterDeadline
    );
}