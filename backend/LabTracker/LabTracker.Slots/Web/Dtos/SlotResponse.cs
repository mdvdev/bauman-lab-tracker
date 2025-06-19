using Courses.Web.Dtos;
using LabTracker.Slots.Domain;
using Users.Web.Dtos;

namespace Slots.Web.Dtos;

public record SlotResponse(
    Guid Id,
    CourseResponse Course,
    UserResponse Teacher,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    int MaxStudents)
{
    public static SlotResponse Create(SlotInfo slotInfo) =>
        new(
            Id: slotInfo.Slot.Id,
            Course: CourseResponse.Create(slotInfo.Course),
            Teacher: UserResponse.Create(slotInfo.Teacher),
            StartTime: slotInfo.Slot.StartTime,
            EndTime: slotInfo.Slot.EndTime,
            MaxStudents: slotInfo.Slot.MaxStudents
        );
}