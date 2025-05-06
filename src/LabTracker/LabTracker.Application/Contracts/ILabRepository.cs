using LabTracker.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LabTracker.Application.Contracts
{
    public interface ILabRepository: ICrudRepository<Lab, Guid>
    {
        Task<Lab?> GetByIdAsync(Guid id);
        Task<IEnumerable<Lab>> GetByCourseIdAsync(Guid courseId);
        Task<IEnumerable<Lab>> GetAllAsync();
        Task CreateAsync(Lab lab);
        Task UpdateAsync(Lab lab);
        Task DeleteAsync(Guid id);
    }
}