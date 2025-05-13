using LabTracker.Application.Abstractions;
using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace LabTracker.Application.Submissions;

public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ILabRepository _labRepository;
    private readonly ISlotRepository _slotRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<SubmissionService> _logger;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        ILabRepository labRepository,
        ISlotRepository slotRepository,
        ICourseRepository courseRepository,
        ILogger<SubmissionService> logger)
    {
        _submissionRepository = submissionRepository;
        _labRepository = labRepository;
        _slotRepository = slotRepository;
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Submission> CreateSubmissionAsync(
        Guid courseId,
        Guid studentId,
        Guid labId,
        Guid slotId)
    {
        var lab = await _labRepository.GetByIdAsync(labId);
        if (lab is null || lab.CourseId != courseId)
        {
            throw new Exception("Lab does not belong to the course");
        }

        var slot = await _slotRepository.GetByIdAsync(slotId);
        if (slot is null)
        {
            throw new Exception("Slot does not exist");
        }

        var currentSubmissions = await _submissionRepository.GetByCourseIdAsync(courseId);
        var slotSubmissions = currentSubmissions.Count(s => s.SlotId == slotId);
        

        var submission = new Submission
        {
            StudentId = studentId,
            LabId = labId,
            SlotId = slotId,
            CourseId = courseId,
            Status = Status.Pending
        };

        await _submissionRepository.CreateAsync(submission);
        return submission;
    }

    public async Task<IEnumerable<Submission>> GetSubmissionsAsync(
        Guid courseId,
        Guid studentId,
        string? status = null)
    {
        return await _submissionRepository.GetByCourseIdAsync(courseId, studentId, status);
    }
    
    public async Task<Submission?> GetSubmissionAsync(Guid courseId, Guid submissionId, Guid userId)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId);
    
        if (submission is null || submission.CourseId != courseId)
            return null;

        return submission;
    }
    

    public async Task DeleteSubmissionAsync(Guid courseId, Guid submissionId)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId);
        if (submission is null || submission.CourseId != courseId)
        {
            throw new Exception("Submission not found or does not belong to the course");
        }

        await _submissionRepository.DeleteAsync(submissionId);
    }

    public async Task<Submission> UpdateSubmissionStatusAsync(
        Guid courseId,
        Guid submissionId,
        Status status,
        int? score,
        string? comment)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId);
        if (submission is null || submission.CourseId != courseId)
        {
            throw new Exception("Submission not found or does not belong to the course");
        }

        submission.Status = status;
        submission.Score = score;
        submission.Comment = comment;
        submission.UpdatedAt = DateTimeOffset.UtcNow;

        await _submissionRepository.UpdateAsync(submission);
        return submission;
    }
}