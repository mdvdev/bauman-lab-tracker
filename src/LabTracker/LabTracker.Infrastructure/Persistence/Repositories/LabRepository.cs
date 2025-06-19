using LabTracker.Infrastructure.Persistence.Entities;
using LabTracker.Labs.Abstractions.Repositories;
using LabTracker.Labs.Domain;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class LabRepository : ILabRepository
{
    private readonly ApplicationDbContext _context;

    public LabRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Lab?> GetByIdAsync(Guid labId)
    {
        var entity = await _context.Labs.FindAsync(labId);
        return entity?.ToDomain();
    }

    public async Task<IEnumerable<Lab>> GetAllAsync()
    {
        var entities = await _context.Labs.ToListAsync();
        return entities.Select(e => e.ToDomain());
    }

    public async Task<IEnumerable<Lab>> GetByCourseIdAsync(Guid courseId)
    {
        var entities = await _context.Labs.Where(l => l.CourseId == courseId).ToListAsync();
        return entities.Select(e => e.ToDomain());
    }

    public async Task<Lab> CreateAsync(Lab lab)
    {
        if (await _context.Labs.FindAsync(lab.Id) is null)
        {
            await _context.Labs.AddAsync(LabEntity.FromDomain(lab));
            await _context.SaveChangesAsync();
        }

        return lab;
    }

    public async Task<Lab> UpdateAsync(Lab lab)
    {
        var entity = await _context.Labs.FindAsync(lab.Id);

        if (entity is null) return await CreateAsync(lab);

        entity.CourseId = lab.CourseId;
        entity.Name = lab.Name;
        entity.DescriptionUri = lab.DescriptionUri;
        entity.Deadline = lab.Deadline;
        entity.Score = lab.Score;
        entity.ScoreAfterDeadline = lab.ScoreAfterDeadline;

        _context.Labs.Update(entity);
        await _context.SaveChangesAsync();

        return lab;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Labs.FindAsync(id);
        if (entity is not null)
        {
            _context.Labs.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}