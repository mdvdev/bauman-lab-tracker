using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Infrastructure.Persistence.Entities;
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
        var entity = await _context.Courses.FindAsync(id);
        return entity?.ToDomain();
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        var entities = await _context.Courses.ToListAsync();
        return entities.Select(e => e.ToDomain());
    }

    public async Task<Guid> CreateAsync(Course course)
    {
        if (await _context.Courses.FindAsync(course.Id) is null)
        {
            await _context.Courses.AddAsync(CourseEntity.FromDomain(course));
            await _context.SaveChangesAsync();
        }

        return course.Id;
    }

    public async Task UpdateAsync(Course course)
    {
        var entity = await _context.Courses.FindAsync(course.Id);
        if (entity is not null)
        {
            entity.Name = course.Name.Value;
            entity.Description = course.Description;
            entity.QueueMode = course.QueueMode;
            entity.PhotoUri = course.PhotoUri;
            _context.Courses.Update(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course is not null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}