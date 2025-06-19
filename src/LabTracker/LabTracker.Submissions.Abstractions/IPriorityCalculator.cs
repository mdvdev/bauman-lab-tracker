namespace LabTracker.Submissions.Abstractions;

public interface IPriorityCalculator
{
    Task<double> CalculatePriorityAsync(Guid studentId, Guid courseId, Guid slotId);   
}