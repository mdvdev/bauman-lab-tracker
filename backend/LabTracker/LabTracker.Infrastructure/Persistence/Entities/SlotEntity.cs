using LabTracker.Slots.Domain;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class SlotEntity
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid TeacherId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public int MaxStudents { get; set; }

    public CourseEntity Course { get; init; }
    public UserEntity Teacher { get; init; }

    public Slot ToDomain()
    {
        return Slot.Restore(
            id: Id,
            courseId: CourseId,
            teacherId: TeacherId,
            startTime: StartTime,
            endTime: EndTime,
            maxStudents: MaxStudents);
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