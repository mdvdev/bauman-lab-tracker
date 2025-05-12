using LabTracker.Domain.Entities;

namespace LabTracker.Presentation.Dtos.Responses;

public class LabResponse
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public int Score { get; set; }
    public int ScoreAfterDeadline { get; set; }

    public static LabResponse Create(Lab lab) => new()
    {
        Id = lab.Id,
        CourseId = lab.CourseId,
        Name = lab.Name,
        Description = lab.Description,
        Deadline = lab.Deadline,
        Score = lab.Score,
        ScoreAfterDeadline = lab.ScoreAfterDeadline
    };
}