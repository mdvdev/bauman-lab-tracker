using LabTracker.Labs.Domain;
using LabTracker.Shared.Contracts;

namespace LabTracker.Labs.Abstractions.Repositories;

public interface ILabRepository: ICrudRepository<Lab, Guid>
{
    public Task<IEnumerable<Lab>> GetByCourseIdAsync(Guid courseId);
}