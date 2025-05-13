using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Contracts;

public interface ISubmissionService
{
    Task<Submission> CreateSubmissionAsync(Guid courseId, Guid studentId, Guid labId, Guid slotId);
    Task<IEnumerable<Submission>> GetSubmissionsAsync(Guid courseId, Guid studentId, string? status = null);
    Task DeleteSubmissionAsync(Guid courseId, Guid submissionId);
    Task<Submission> UpdateSubmissionStatusAsync(Guid courseId, Guid submissionId, Status status, int? score, string? comment);
}