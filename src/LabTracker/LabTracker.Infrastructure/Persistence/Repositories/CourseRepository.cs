using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses.FindAsync(id);
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _context.Courses.ToListAsync();
    }

    public async Task CreateAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        var entry = _context.Entry(course);
        if (entry.State == EntityState.Detached)
        {
            throw new InvalidOperationException("Entity is not being tracked.");
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course is null) throw new KeyNotFoundException($"Course with id '{id}' not found.");

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
    }
}