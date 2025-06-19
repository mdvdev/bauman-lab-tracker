using LabTracker.Infrastructure.Persistence.Entities;
using LabTracker.Submissions.Abstractions.Repositories;
using LabTracker.Submissions.Domain;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class OligarchStudentRepository : IOligarchStudentRepository
{
    private readonly ApplicationDbContext _context;

    public OligarchStudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsOligarchAsync(Guid studentId, Guid courseId)
    {
        return await _context.OligarchStudents.FindAsync(studentId, courseId) is not null;
    }

    public async Task<IEnumerable<OligarchStudent>> GetAllOligarchsAsync(Guid courseId)
    {
        var students = await _context.OligarchStudents
            .Where(s => s.CourseId == courseId)
            .ToListAsync();
        return students.Select(s => s.ToDomain());
    }

    public async Task CreateAsync(OligarchStudent oligarch)
    {
        await _context.AddAsync(OligarchStudentEntity.FromDomain(oligarch));
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid studentId, Guid courseId)
    {
        var entity = await _context.OligarchStudents.FindAsync(studentId, courseId);
        if (entity is not null)
        {
            _context.OligarchStudents.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}