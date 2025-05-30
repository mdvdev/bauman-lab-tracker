using LabTracker.Submissions.Abstractions;

namespace LabTracker.Submissions;

public class AnarchicPriorityCalculator : IPriorityCalculator
{
    private static readonly Random Random = new();

    public Task<double> CalculatePriorityAsync(Guid studentId, Guid courseId, Guid slotId)
    {
        // Priority is in range [0.0, 1.0)
        var randomPriority = Random.NextDouble();
        return Task.FromResult(randomPriority);
    }
}