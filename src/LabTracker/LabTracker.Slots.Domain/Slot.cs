namespace LabTracker.Slots.Domain;

public class Slot
{
    public Guid Id { get; }
    public Guid CourseId { get; private set; }
    public Guid TeacherId { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public int MaxStudents { get; private set; }

    private Slot(
        Guid id,
        Guid courseId,
        Guid teacherId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        int maxStudents)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course ID cannot be empty.", nameof(courseId));

        if (teacherId == Guid.Empty)
            throw new ArgumentException("Teacher ID cannot be empty.", nameof(teacherId));

        if (startTime >= endTime)
            throw new ArgumentException("Start time must be earlier than end time.");

        if (maxStudents <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxStudents), "Max students must be greater than zero.");

        Id = id;
        CourseId = courseId;
        TeacherId = teacherId;
        StartTime = startTime;
        EndTime = endTime;
        MaxStudents = maxStudents;
    }

    public static Slot CreateNew(
        Guid courseId,
        Guid teacherId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        int maxStudents)
    {
        return new Slot(
            Guid.NewGuid(),
            courseId,
            teacherId,
            startTime,
            endTime,
            maxStudents);
    }

    public static Slot Restore(
        Guid id,
        Guid courseId,
        Guid teacherId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        int maxStudents)
    {
        return new Slot(
            id,
            courseId,
            teacherId,
            startTime,
            endTime,
            maxStudents);
    }

    public void Update(
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        int? maxStudents = null)
    {
        if (startTime is not null && endTime is not null && startTime >= endTime)
            throw new ArgumentException("Start time must be earlier than end time.");

        if (startTime is not null)
            StartTime = startTime.Value;

        if (endTime is not null)
            EndTime = endTime.Value;

        if (maxStudents is not null)
        {
            if (maxStudents <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxStudents), "Max students must be greater than zero.");

            MaxStudents = maxStudents.Value;
        }
    }
}