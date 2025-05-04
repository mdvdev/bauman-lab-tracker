namespace LabTracker.Domain.Entities;

public class CourseMember
{
    public Guid CourseId { get; set; }
    public Guid MemberId { get; set; }
    public DateTimeOffset AssignedAt { get; set; } = DateTimeOffset.UtcNow;
    public Course Course { get; set; }
    public User User { get; set; }

    private CourseMember()
    {
    }

    public CourseMember(Course course, User user)
    {
        CourseId = course.Id;
        MemberId = user.Id;
        Course = course;
        User = user;
    }
}