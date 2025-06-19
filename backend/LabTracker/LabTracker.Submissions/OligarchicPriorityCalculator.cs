using LabTracker.CourseMembers.Abstractions.Repositories;
using LabTracker.Slots.Abstractions.Repositories;
using LabTracker.Submissions.Abstractions;
using LabTracker.Submissions.Abstractions.Repositories;

namespace LabTracker.Submissions;

public class OligarchicPriorityCalculator : IPriorityCalculator
{
    private readonly ICourseMemberRepository _courseMemberRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ISlotRepository _slotRepository;
    private readonly IOligarchStudentRepository _oligarchStudentRepository;

    private const double KOligarch = 0.25;

    public OligarchicPriorityCalculator(
        ICourseMemberRepository courseMemberRepository,
        ISubmissionRepository submissionRepository,
        ISlotRepository slotRepository,
        IOligarchStudentRepository oligarchStudentRepository)
    {
        _courseMemberRepository = courseMemberRepository;
        _submissionRepository = submissionRepository;
        _slotRepository = slotRepository;
        _oligarchStudentRepository = oligarchStudentRepository;
    }

    public async Task<double> CalculatePriorityAsync(Guid studentId, Guid courseId, Guid slotId)
    {
        var democraticPriorityCalculator =
            new DemocraticPriorityCalculator(_courseMemberRepository, _submissionRepository, _slotRepository);

        var priority = await democraticPriorityCalculator.CalculatePriorityAsync(studentId, courseId, slotId);

        if (await _oligarchStudentRepository.IsOligarchAsync(studentId, courseId))
            priority += KOligarch;

        return priority;
    }
}