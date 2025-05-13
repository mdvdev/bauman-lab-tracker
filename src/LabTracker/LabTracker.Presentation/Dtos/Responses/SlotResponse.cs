using LabTracker.Domain.Entities;

namespace LabTracker.Presentation.Dtos.Responses;

public class SlotResponse
{
    public required Guid Id { get; init; }
    public required CourseResponse Course { get; init; }
    public required UserResponse Teacher { get; init; }
    public required DateTimeOffset StartTime { get; set; }
    public required DateTimeOffset EndTime { get; set; }
    public required int MaxStudents { get; set; }

    public static SlotResponse Create(Slot slot, Course course, User user)
    {
        return new SlotResponse
        {
            Id = slot.Id,
            Course = CourseResponse.Create(course),
            Teacher = UserResponse.Create(user),
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            MaxStudents = slot.MaxStudents
        };
    }
}