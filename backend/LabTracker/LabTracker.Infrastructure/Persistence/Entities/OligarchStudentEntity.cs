using LabTracker.Submissions.Domain;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class OligarchStudentEntity
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public DateTimeOffset GrantedAt { get; set; }
    public UserEntity User { get; set; }
    public CourseEntity Course { get; set; }

    public OligarchStudent ToDomain()
    {
        return OligarchStudent.Restore(UserId, CourseId, GrantedAt);
    }

    public static OligarchStudentEntity FromDomain(OligarchStudent oligarch)
    {
        return new OligarchStudentEntity
        {
            UserId = oligarch.UserId,
            CourseId = oligarch.CourseId,
            GrantedAt = oligarch.GrantedAt
        };
    }
}