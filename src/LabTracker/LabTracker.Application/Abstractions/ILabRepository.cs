using LabTracker.Domain.Entities;

namespace LabTracker.Application.Contracts;

public interface ILabRepository: ICrudRepository<Lab, Guid>
{
    public Task<IEnumerable<Lab>> GetByCourseIdAsync(Guid courseId);
}