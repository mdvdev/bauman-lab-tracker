namespace LabTracker.Domain.Entities;

public class Lab
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public int Score { get; set; }
    public int ScoreAfterDeadline { get; set; }

}