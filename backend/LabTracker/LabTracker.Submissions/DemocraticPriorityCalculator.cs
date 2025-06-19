using LabTracker.CourseMembers.Abstractions.Repositories;
using LabTracker.Slots.Abstractions.Repositories;
using LabTracker.Submissions.Abstractions;
using LabTracker.Submissions.Abstractions.Repositories;
using LabTracker.Submissions.Domain;

namespace LabTracker.Submissions;

public class DemocraticPriorityCalculator : IPriorityCalculator
{
    private readonly ICourseMemberRepository _courseMemberRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ISlotRepository _slotRepository;

    private const double KScore = 0.4;
    private const double KInactivity = 0.4;
    private const double KQueueBonus = 0.2;
    private const double Epsilon = 1e-9;

    public DemocraticPriorityCalculator(
        ICourseMemberRepository courseMemberRepository,
        ISubmissionRepository submissionRepository,
        ISlotRepository slotRepository)
    {
        _courseMemberRepository = courseMemberRepository;
        _submissionRepository = submissionRepository;
        _slotRepository = slotRepository;
    }

    public async Task<double> CalculatePriorityAsync(Guid studentId, Guid courseId, Guid slotId)
    {
        var today = DateTime.UtcNow.Date;

        var students = (await _courseMemberRepository.GetStudentsByCourseIdAsync(courseId))
            .Where(s => s.Score.HasValue)
            .ToList();

        var currentStudent = students.SingleOrDefault(s => s.Id.UserId == studentId)
                             ?? throw new ArgumentException(
                                 $"Student {studentId} not found in course {courseId}.");

        var submissions = (await _submissionRepository.GetAllAsync()).ToList();

        var currentStudentSubmissions = submissions
            .Where(s => s.StudentId == studentId && s.SubmissionStatus == SubmissionStatus.Approved)
            .ToList();

        // 1. Score priority.
        double scorePriority = 0;
        if (students.Count > 1)
        {
            var max = students.Max(s => s.Score!.Value);
            var min = students.Min(s => s.Score!.Value);
            if (max != min)
                scorePriority = KScore * (currentStudent.Score!.Value - min) / (max - min);
        }

        // 2. Inactivity priority.
        double inactivityPriority = 0;
        var approvedSubmissions = submissions
            .Where(s => s.SubmissionStatus == SubmissionStatus.Approved)
            .ToList();

        if (approvedSubmissions.Count > 1)
        {
            var maxDaysAgo = approvedSubmissions.Max(s => (today - s.CreatedAt.Date).TotalDays);
            var minDaysAgo = approvedSubmissions.Min(s => (today - s.CreatedAt.Date).TotalDays);

            double daysSinceLast = currentStudentSubmissions.Count > 0
                ? (today - currentStudentSubmissions.Max(s => s.CreatedAt.Date)).TotalDays
                : maxDaysAgo;

            if (Math.Abs(maxDaysAgo - minDaysAgo) > Epsilon)
                inactivityPriority = KInactivity * (daysSinceLast - minDaysAgo) / (maxDaysAgo - minDaysAgo);
        }

        // 3. Queue bonus.
        double queueBonus = 0;
        var previousSlot = await _slotRepository.GetPreviousSlotAsync(slotId);
        if (previousSlot != null)
        {
            var wasQueued = submissions.Any(s =>
                s.SlotId == previousSlot.Id &&
                s.StudentId == studentId &&
                s.SubmissionStatus != SubmissionStatus.Approved);

            queueBonus = wasQueued ? KQueueBonus : 0;
        }

        var priority =  scorePriority + inactivityPriority + queueBonus;

        return priority;
    }
}