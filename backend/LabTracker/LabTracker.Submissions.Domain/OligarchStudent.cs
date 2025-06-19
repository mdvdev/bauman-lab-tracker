namespace LabTracker.Submissions.Domain;

public class OligarchStudent
{
    public Guid UserId { get; }
    public Guid CourseId { get; }
    public DateTimeOffset GrantedAt { get; private set; }

    private OligarchStudent(Guid userId, Guid courseId, DateTimeOffset grantedAt)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (courseId == Guid.Empty) throw new ArgumentException("CourseId cannot be empty.", nameof(courseId));

        UserId = userId;
        CourseId = courseId;
        GrantedAt = grantedAt;
    }

    public static OligarchStudent CreateNew(Guid userId, Guid courseId)
    {
        return new OligarchStudent(userId, courseId, DateTimeOffset.UtcNow);
    }

    public static OligarchStudent Restore(Guid userId, Guid courseId, DateTimeOffset grantedAt)
    {
        return new OligarchStudent(userId, courseId, grantedAt);
    }

    public void UpdateGrantedAt(DateTimeOffset newGrantedAt)
    {
        GrantedAt = newGrantedAt;
    }
}