using LabTracker.Domain.Entities;

namespace LabTracker.Application.Contracts;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(Guid id);
    Task<IEnumerable<Submission>> GetByCourseIdAsync(Guid courseId, Guid? studentId = null, string? status = null);
    Task<Guid> CreateAsync(Submission submission);
    Task UpdateAsync(Submission submission);
    Task DeleteAsync(Guid id);
}