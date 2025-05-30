using LabTracker.Slots.Abstractions.Services.Dtos;
using LabTracker.Slots.Domain;

namespace LabTracker.Slots.Abstractions.Services;

public interface ISlotService
{
    Task<SlotInfo> CreateSlotAsync(Guid courseId, CreateSlotRequest request);
    Task<SlotInfo?> GetSlotAsync(Guid slotId);
    Task<IEnumerable<SlotInfo>> GetCourseSlotsAsync(Guid courseId);
    Task UpdateSlotAsync(Guid slotId, UpdateSlotRequest request);
    Task DeleteSlotAsync(Guid slotId);
}