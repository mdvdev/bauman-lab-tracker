using LabTracker.Submissions.Domain;

namespace LabTracker.Submissions.Abstractions.Repositories;

public interface IOligarchStudentRepository
{
    Task<bool> IsOligarchAsync(Guid studentId, Guid courseId);
    Task<IEnumerable<OligarchStudent>> GetAllOligarchsAsync(Guid courseId);
    Task CreateAsync(OligarchStudent oligarch);
    Task DeleteAsync(Guid studentId, Guid courseId);
}