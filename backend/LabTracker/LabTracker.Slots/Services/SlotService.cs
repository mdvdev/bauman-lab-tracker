using LabTracker.Courses.Abstractions.Repositories;
using LabTracker.Slots.Abstractions.Repositories;
using LabTracker.Slots.Abstractions.Services;
using LabTracker.Slots.Abstractions.Services.Dtos;
using LabTracker.Slots.Domain;
using LabTracker.User.Abstractions.Repositories;

namespace Slots.Services;

public class SlotService : ISlotService
{
    private readonly ISlotRepository _slotRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    private const int MaxStudentsLimit = 100;

    public SlotService(
        ISlotRepository slotRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _slotRepository = slotRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<SlotInfo> CreateSlotAsync(Guid courseId, CreateSlotRequest request)
    {
        var course = await _courseRepository.GetByIdAsync(courseId)
                     ?? throw new KeyNotFoundException($"Course {courseId} not found.");

        var teacher = await _userRepository.GetByIdAsync(request.TeacherId)
                      ?? throw new KeyNotFoundException($"Teacher {request.TeacherId} not found.");

        if (request.MaxStudents > MaxStudentsLimit)
            throw new ArgumentOutOfRangeException(
                $"Max students limit exceeded. Limit is '{MaxStudentsLimit}'.");

        var slot = Slot.CreateNew(
            courseId: courseId,
            teacherId: request.TeacherId,
            startTime: request.StartTime,
            endTime: request.EndTime,
            maxStudents: request.MaxStudents
        );

        if (_slotRepository.IsIntervalsOverlapping(courseId, slot))
            throw new ArgumentException("Invalid slot interval: intervals overlap.");

        await _slotRepository.CreateAsync(slot);

        return new SlotInfo(slot, course, teacher);
    }

    public async Task<SlotInfo?> GetSlotAsync(Guid slotId)
    {
        var slot = await _slotRepository.GetByIdAsync(slotId)
                   ?? throw new KeyNotFoundException($"Slot with id '{slotId}' not found.");

        var course = await _courseRepository.GetByIdAsync(slot.CourseId)
                     ?? throw new InvalidOperationException($"Course {slot.CourseId} not found.");

        var teacher = await _userRepository.GetByIdAsync(slot.TeacherId)
                      ?? throw new InvalidOperationException($"Teacher {slot.TeacherId} not found.");

        return new SlotInfo(slot, course, teacher);
    }

    public async Task<IEnumerable<SlotInfo>> GetCourseSlotsAsync(Guid courseId)
    {
        var slots = await _slotRepository.GetCourseSlotsAsync(courseId);
        var course = await _courseRepository.GetByIdAsync(courseId)
                     ?? throw new InvalidOperationException($"Course {courseId} not found.");

        var result = new List<SlotInfo>();

        foreach (var slot in slots)
        {
            var teacher = await _userRepository.GetByIdAsync(slot.TeacherId)
                          ?? throw new InvalidOperationException($"Teacher {slot.TeacherId} not found.");

            result.Add(new SlotInfo(slot, course, teacher));
        }

        return result;
    }

    public async Task UpdateSlotAsync(Guid slotId, UpdateSlotRequest request)
    {
        var slot = await _slotRepository.GetByIdAsync(slotId);
        if (slot is null)
            throw new KeyNotFoundException($"Slot with id {slotId} not found.");

        slot.Update(
            startTime: request.StartTime,
            endTime: request.EndTime,
            maxStudents: request.MaxStudents);

        await _slotRepository.UpdateAsync(slot);
    }

    public async Task DeleteSlotAsync(Guid slotId)
    {
        if (await _slotRepository.GetByIdAsync(slotId) is null)
            throw new KeyNotFoundException($"Slot with id '{slotId}' not found.");

        await _slotRepository.DeleteAsync(slotId);
    }
}