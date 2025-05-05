using LabTracker.Domain.Entities;

namespace LabTracker.Application.Contracts.Labs
{
    public interface ILabService
    {
        Task<Lab> CreateLabAsync(Guid courseId, string name, string description, DateTimeOffset deadline, int score, int scoreAfterDeadline);
        Task DeleteLabAsync(Guid id);
        Task UpdateLabAsync(Guid id, string name, string description, DateTimeOffset deadline, int score, int scoreAfterDeadline);
        Task<Lab?> GetLabByIdAsync(Guid id);
        Task<IEnumerable<Lab>> GetLabsByCourseIdAsync(Guid courseId);
    }
}