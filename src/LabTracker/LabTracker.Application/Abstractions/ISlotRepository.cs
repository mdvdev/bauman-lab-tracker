using LabTracker.Domain.Entities;

namespace LabTracker.Application.Abstractions;

public interface ISlotRepository : ICrudRepository<Slot, Guid>
{
    bool IsIntervalsOverlapping(Slot slot);
    Task<IEnumerable<Slot>> GetAllCourseSlotsAsync(Guid courseId);
}