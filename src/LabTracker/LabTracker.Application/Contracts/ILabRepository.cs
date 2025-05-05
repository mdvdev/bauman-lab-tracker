using LabTracker.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LabTracker.Application.Contracts
{
    public interface ILabRepository
    {
        Task<Lab?> GetByIdAsync(Guid id);
        Task<IEnumerable<Lab>> GetByCourseIdAsync(Guid courseId);
        Task CreateAsync(Lab lab);
        Task UpdateAsync(Lab lab);
        Task DeleteAsync(Guid id);
    }
}