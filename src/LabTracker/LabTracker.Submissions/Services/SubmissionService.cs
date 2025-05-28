using LabTracker.CourseMembers.Abstractions.Services;
using LabTracker.Courses.Abstractions.Repositories;
using LabTracker.Labs.Abstractions.Repositories;
using LabTracker.Notifications.Abstractions.Services;
using LabTracker.Slots.Abstractions.Services;
using LabTracker.Submissions.Abstractions;
using LabTracker.Submissions.Abstractions.Repositories;
using LabTracker.Submissions.Abstractions.Services;
using LabTracker.Submissions.Abstractions.Services.Dtos;
using LabTracker.Submissions.Domain;
using LabTracker.User.Abstractions.Repositories;

namespace LabTracker.Submissions.Services;

public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ILabRepository _labRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseMemberService _courseMemberService;
    private readonly ISlotService _slotService;
    private readonly IPriorityCalculatorFactory _priorityCalculatorFactory;
    private readonly INotificationService _notificationService;
    

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        ILabRepository labRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository,
        ICourseMemberService courseMemberService,
        ISlotService slotService,
        IPriorityCalculatorFactory priorityCalculatorFactory,
        INotificationService notificationService)
    {
        _submissionRepository = submissionRepository;
        _labRepository = labRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
        _courseMemberService = courseMemberService;
        _slotService = slotService;
        _priorityCalculatorFactory = priorityCalculatorFactory;
        _notificationService = notificationService;
    }

    public async Task<SubmissionInfo> CreateSubmissionAsync(Guid courseId, CreateSubmissionRequest request)
    {
        var course = await _courseRepository.GetByIdAsync(courseId)
                     ?? throw new ArgumentException($"Course with id '{courseId}' does not exist.", nameof(courseId));
        
        var priorityCalculator = _priorityCalculatorFactory.GetPriorityCalculator(course.QueueMode);
        var priority = await priorityCalculator.CalculatePriorityAsync(request.StudentId, courseId, request.SlotId);
        
        var submission = Submission.CreateNew(
            studentId: request.StudentId,
            labId: request.LabId,
            slotId: request.SlotId,
            courseId: courseId,
            priority: priority);

        var info = await LoadSubmissionContextAsync(
            courseId,
            request.StudentId,
            request.LabId,
            request.SlotId,
            submission,
            validateInput: true);

        await _submissionRepository.CreateAsync(submission);

        return info;
    }

    public async Task<IEnumerable<SubmissionInfo>> GetCourseSubmissionsAsync(
        Guid courseId,
        Func<SubmissionInfo, bool>? predicate = null)
    {
        var submissions = await _submissionRepository.GetByCourseIdAsync(courseId);
        var result = new List<SubmissionInfo>();

        foreach (var s in submissions)
        {
            var submissionInfo = await CreateSubmissionInfoAsync(s);
            
            if (predicate is null || predicate(submissionInfo))
                result.Add(submissionInfo);
        }

        return result
            .OrderByDescending(s => s.Submission.Priority)
            .ThenBy(s => s.Submission.CreatedAt);;
    }

    public async Task<SubmissionInfo?> GetSubmissionAsync(Guid submissionId)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId);
        if (submission is null)
            return null;

        return await CreateSubmissionInfoAsync(submission);
    }

    public async Task DeleteSubmissionAsync(Guid submissionId)
    {
        if (await _submissionRepository.GetByIdAsync(submissionId) is null)
            throw new KeyNotFoundException($"Submission with id '{submissionId}' not found.");

        await _submissionRepository.DeleteAsync(submissionId);
    }

    public async Task<SubmissionInfo> UpdateSubmissionStatusAsync(
        Guid submissionId,
        UpdateSubmissionStatusRequest request)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId)
                         ?? throw new KeyNotFoundException($"Submission with id '{submissionId}' not found.");

        submission.UpdateStatus(
            newSubmissionStatus: request.SubmissionStatus,
            score: request.Score,
            comment: request.Comment);

        await _submissionRepository.UpdateAsync(submission);
        
        return await CreateSubmissionInfoAsync(submission);
    }

    private async Task<SubmissionInfo> CreateSubmissionInfoAsync(Submission submission)
    {
        return await LoadSubmissionContextAsync(
            submission.CourseId,
            submission.StudentId,
            submission.LabId,
            submission.SlotId,
            submission,
            validateInput: false);
    }

    private async Task<SubmissionInfo> LoadSubmissionContextAsync(
        Guid courseId,
        Guid studentId,
        Guid labId,
        Guid slotId,
        Submission submission,
        bool validateInput = false)
    {
        var course = await _courseRepository.GetByIdAsync(courseId)
                     ?? throw CreateException("Course", courseId, validateInput);

        var lab = await _labRepository.GetByIdAsync(labId);
        if (lab is null || lab.CourseId != courseId)
            throw CreateException("Lab", labId, validateInput);

        var slotInfo = await _slotService.GetSlotAsync(slotId);
        if (slotInfo is null || slotInfo.Slot.CourseId != courseId)
            throw CreateException("Slot", slotId, validateInput);

        var student = await _userRepository.GetByIdAsync(studentId)
                      ?? throw CreateException("Student", studentId, validateInput);

        if (!await _courseMemberService.IsCourseMemberAsync(new(courseId, studentId)))
            throw CreateException($"Student '{studentId}' is not a member of course '{courseId}'", validateInput);

        return new SubmissionInfo(
            Student: student,
            Lab: lab,
            SlotInfo: slotInfo,
            Course: course,
            Submission: submission);
    }

    private Exception CreateException(string entityName, Guid id, bool isInput)
    {
        var message = $"{entityName} with ID '{id}' does not exist.";
        return isInput ? new ArgumentException(message) : new InvalidOperationException(message);
    }

    private Exception CreateException(string message, bool isInput)
    {
        return isInput ? new ArgumentException(message) : new InvalidOperationException(message);
    }
}