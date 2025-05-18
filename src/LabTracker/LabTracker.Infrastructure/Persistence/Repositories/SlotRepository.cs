using LabTracker.Infrastructure.Persistence.Entities;
using LabTracker.Slots.Abstractions.Repositories;
using LabTracker.Slots.Domain;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class SlotRepository : ISlotRepository
{
    private readonly ApplicationDbContext _context;

    public SlotRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Slot?> GetByIdAsync(Guid labId)
    {
        var entity = await _context.Slots.FindAsync(labId);
        return entity?.ToDomain();
    }

    public async Task<IEnumerable<Slot>> GetAllAsync()
    {
        var entities = await _context.Slots.ToListAsync();
        return entities.Select(e => e.ToDomain());
    }

    public async Task<Slot> CreateAsync(Slot slot)
    {
        if (await _context.Slots.FindAsync(slot.Id) is null)
        {
            await _context.Slots.AddAsync(SlotEntity.FromDomain(slot));
            await _context.SaveChangesAsync();
        }

        return slot;
    }

    public async Task<Slot> UpdateAsync(Slot slot)
    {
        var entity = await _context.Slots.FindAsync(slot.Id);

        if (entity is null) return await CreateAsync(slot);

        entity.CourseId = slot.CourseId;
        entity.TeacherId = slot.TeacherId;
        entity.StartTime = slot.StartTime;
        entity.EndTime = slot.EndTime;
        entity.MaxStudents = slot.MaxStudents;

        _context.Slots.Update(entity);
        await _context.SaveChangesAsync();

        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid key)
    {
        var entity = await _context.Slots.FindAsync(key);
        if (entity is not null)
        {
            _context.Slots.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public bool IsIntervalsOverlapping(Slot slot)
    {
        return Enumerable.Any(
            _context.Slots, s => s.StartTime <= slot.EndTime && s.EndTime >= slot.StartTime
        );
    }

    public async Task<IEnumerable<Slot>> GetCourseSlotsAsync(Guid courseId)
    {
        return await _context.Slots
            .Where(s => s.CourseId == courseId)
            .Select(e => e.ToDomain())
            .ToListAsync();
    }
}