namespace LabTracker.CourseMembers.Domain;

public class CourseMember
{
    public CourseMemberKey Id { get; }
    public DateTimeOffset AssignedAt { get; }
    public int? Score { get; private set; }

    private CourseMember(CourseMemberKey id, DateTimeOffset assignedAt, int? score)
    {
        Id = id;
        AssignedAt = assignedAt;
        Score = score;
    }

    public static CourseMember CreateNew(CourseMemberKey key, int? score)
    {
        return new CourseMember(key, DateTimeOffset.UtcNow, score);
    }

    public static CourseMember Restore(CourseMemberKey id, DateTimeOffset assignedAt, int? score)
    {
        return new CourseMember(id, assignedAt, score);
    }

    public void AddScore(int score)
    {
        if (score < 0) throw new ArgumentOutOfRangeException(nameof(score), "Score must be positive.");
        Score ??= 0;
        Score += score;
    }
}