using LabTracker.Labs.Abstractions.Services.Dtos;
using LabTracker.Labs.Domain;

namespace LabTracker.Labs.Abstractions.Services;

public interface ILabService
{
    Task<Lab> CreateLabAsync(Guid courseId, CreateLabRequest request);
    Task DeleteLabAsync(Guid labId);
    Task UpdateLabAsync(Guid labId, UpdateLabRequest request);
    Task UpdateLabDescriptionAsync(Guid labId, Stream stream, string fileName);
    Task<Lab?> GetLabByIdAsync(Guid labId);
    Task<IEnumerable<Lab>> GetLabsByCourseIdAsync(Guid courseId);
}