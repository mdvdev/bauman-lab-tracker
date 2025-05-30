using LabTracker.Submissions.Abstractions.Services.Dtos;
using LabTracker.Submissions.Domain;

namespace LabTracker.Submissions.Abstractions.Services;

public interface ISubmissionService
{
    Task<SubmissionInfo> CreateSubmissionAsync(Guid courseId, CreateSubmissionRequest request);
    Task<SubmissionInfo?> GetSubmissionAsync(Guid submissionId);

    Task<IEnumerable<SubmissionInfo>> GetCourseSubmissionsAsync(Guid courseId,
        Func<SubmissionInfo, bool>? predicate = null);

    Task DeleteSubmissionAsync(Guid submissionId);
    Task<SubmissionInfo> UpdateSubmissionStatusAsync(Guid submissionId, UpdateSubmissionStatusRequest request);
}