using LabTracker.Courses.Domain;
using LabTracker.Submissions.Abstractions.Services;

namespace LabTracker.Submissions.Abstractions;

public interface IPriorityCalculatorFactory
{
    IPriorityCalculator GetPriorityCalculator(QueueMode queueMode);
}