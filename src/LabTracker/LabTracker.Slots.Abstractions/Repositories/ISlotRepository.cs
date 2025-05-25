using LabTracker.Shared.Contracts;
using LabTracker.Slots.Domain;

namespace LabTracker.Slots.Abstractions.Repositories;

public interface ISlotRepository : ICrudRepository<Slot, Guid>
{
    bool IsIntervalsOverlapping(Slot slot);
    Task<IEnumerable<Slot>> GetCourseSlotsAsync(Guid courseId);
    public Task<Slot?> GetPreviousSlotAsync(Guid currentSlotId);
}