using LabTracker.Shared.Contracts;
using LabTracker.Submissions.Domain;

namespace LabTracker.Submissions.Abstractions.Repositories;

public interface ISubmissionRepository : ICrudRepository<Submission, Guid>
{
    Task<IEnumerable<Submission>> GetByCourseIdAsync(Guid courseId);
}