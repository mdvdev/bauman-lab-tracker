using LabTracker.Application.Courses.Slots;

namespace LabTracker.Presentation.Dtos.Requests;

public class CreateSlotRequest
{
    public required DateTimeOffset StartTime { get; set; }
    public required DateTimeOffset EndTime { get; set; }
    public required int MaxStudents { get; set; }

    public CreateSlotCommand ToCommand(Guid courseId, Guid teacherId)
    {
        return new CreateSlotCommand(courseId, teacherId, StartTime, EndTime, MaxStudents);
    }
}