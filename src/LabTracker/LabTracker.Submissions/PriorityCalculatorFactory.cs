using LabTracker.CourseMembers.Abstractions.Repositories;
using LabTracker.Courses.Domain;
using LabTracker.Slots.Abstractions.Repositories;
using LabTracker.Submissions.Abstractions;
using LabTracker.Submissions.Abstractions.Repositories;

namespace LabTracker.Submissions;

public class PriorityCalculatorFactory : IPriorityCalculatorFactory
{
    private readonly ICourseMemberRepository _courseMemberRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ISlotRepository _slotRepository;
    private readonly IOligarchStudentRepository _oligarchStudentRepository;

    public PriorityCalculatorFactory(
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

    public IPriorityCalculator GetPriorityCalculator(QueueMode queueMode)
    {
        return queueMode switch
        {
            QueueMode.Anarchic => new AnarchicPriorityCalculator(),
            QueueMode.Democratic => new DemocraticPriorityCalculator(_courseMemberRepository, _submissionRepository,
                _slotRepository),
            QueueMode.Oligarchic => new OligarchicPriorityCalculator(_courseMemberRepository, _submissionRepository,
                _slotRepository, _oligarchStudentRepository),
            _ => throw new ArgumentOutOfRangeException(nameof(queueMode), queueMode, null)
        };
    }
}