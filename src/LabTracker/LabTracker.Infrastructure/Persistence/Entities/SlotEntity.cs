using LabTracker.Domain.Entities;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class SlotEntity
{
    public required Guid Id { get; set; }
    public required Guid CourseId { get; set; }
    public required Guid TeacherId { get; set; }
    public required DateTimeOffset StartTime { get; set; }
    public required DateTimeOffset EndTime { get; set; }
    public required int MaxStudents { get; set; }

    public CourseEntity Course { get; init; }
    public UserEntity Teacher { get; init; }

    public Slot ToDomain()
    {
        return new Slot
        {
            Id = Id,
            CourseId = CourseId,
            TeacherId = TeacherId,
            StartTime = StartTime,
            EndTime = EndTime,
            MaxStudents = MaxStudents
        };
    }

    public static SlotEntity FromDomain(Slot slot)
    {
        return new SlotEntity
        {
            Id = slot.Id,
            CourseId = slot.CourseId,
            TeacherId = slot.TeacherId,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            MaxStudents = slot.MaxStudents,
        };
    }
}