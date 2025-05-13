using LabTracker.Domain.Entities;

namespace LabTracker.Application.Courses.Slots;

public interface ISlotService
{
    Task<Slot> CreateSlotAsync(CreateSlotCommand command);
    Task<Slot?> GetSlotAsync(Guid slotId);
    Task<IEnumerable<Slot>> GetAllCourseSlotsAsync(Guid courseId);
    Task UpdateSlotAsync(UpdateSlotCommand command);
    Task DeleteSlotAsync(Guid slotId);
}