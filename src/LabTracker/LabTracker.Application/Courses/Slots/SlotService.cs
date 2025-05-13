using LabTracker.Application.Abstractions;
using LabTracker.Domain.Entities;

namespace LabTracker.Application.Courses.Slots;

public class SlotService : ISlotService
{
    private readonly ISlotRepository _slotRepository;
    private const int MaxStudentsLimit = 100;

    public SlotService(ISlotRepository slotRepository)
    {
        _slotRepository = slotRepository;
    }

    public async Task<Slot> CreateSlotAsync(CreateSlotCommand command)
    {
        if (command.MaxStudents > MaxStudentsLimit)
            throw new ArgumentOutOfRangeException(
                $"Max students limit exceeded. Limit is '{MaxStudentsLimit}'.");

        var slot = new Slot
        {
            CourseId = command.CourseId,
            TeacherId = command.TeacherId,
            StartTime = command.StartTime,
            EndTime = command.EndTime,
            MaxStudents = command.MaxStudents,
        };

        if (_slotRepository.IsIntervalsOverlapping(slot))
            throw new ArgumentException("Invalid slot interval: intervals overlap.");

        return await _slotRepository.CreateAsync(slot);
    }

    public async Task<Slot?> GetSlotAsync(Guid slotId)
    {
        return await _slotRepository.GetByIdAsync(slotId);
    }

    public Task<IEnumerable<Slot>> GetAllCourseSlotsAsync(Guid courseId)
    {
        return _slotRepository.GetAllCourseSlotsAsync(courseId);
    }

    public async Task UpdateSlotAsync(UpdateSlotCommand command)
    {
        var slot = await _slotRepository.GetByIdAsync(command.SlotId);
        if (slot is null)
            throw new KeyNotFoundException($"Slot with id {command.SlotId} not found.");

        if (command.StartTime is { } startTime)
            slot.StartTime = startTime;

        if (command.EndTime is { } endTime)
            slot.EndTime = endTime;

        if (command.MaxStudents is { } maxStudents)
            slot.MaxStudents = maxStudents;

        await _slotRepository.UpdateAsync(slot);
    }

    public async Task DeleteSlotAsync(Guid slotId)
    {
        if (await _slotRepository.GetByIdAsync(slotId) is null)
            throw new KeyNotFoundException($"Slot with id '{slotId}' not found.");

        await _slotRepository.DeleteAsync(slotId);
    }
}