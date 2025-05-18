using LabTracker.Courses.Abstractions.Repositories;
using LabTracker.Courses.Domain;
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

    public async Task<Course?> GetByIdAsync(Guid labId)
    {
        var entity = await _context.Courses.FindAsync(labId);
        return entity?.ToDomain();
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        var entities = await _context.Courses.ToListAsync();
        return entities.Select(e => e.ToDomain());
    }

    public async Task<Course> CreateAsync(Course course)
    {
        if (await _context.Courses.FindAsync(course.Id) is null)
        {
            await _context.Courses.AddAsync(CourseEntity.FromDomain(course));
            await _context.SaveChangesAsync();
        }

        return course;
    }

    public async Task<Course> UpdateAsync(Course course)
    {
        var entity = await _context.Courses.FindAsync(course.Id);
        if (entity is null) return await CreateAsync(course);

        entity.Name = course.Name;
        entity.Description = course.Description;
        entity.QueueMode = course.QueueMode;
        entity.PhotoUri = course.PhotoUri;

        _context.Courses.Update(entity);
        await _context.SaveChangesAsync();

        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Courses.FindAsync(id);
        if (entity is not null)
        {
            _context.Courses.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Retrieves all courses where the specified user is a member.
    /// Ensures referential integrity by validating existence of each course associated with the user.
    /// </summary>
    /// <param name="userId">The ID of the user whose courses should be retrieved.</param>
    /// <returns>A collection of <see cref="Course"/> instances the user is a member of.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a course referenced by a membership does not exist in the database,
    /// indicating potential referential integrity violation.
    /// </exception>
    public async Task<IEnumerable<Course>> GetCoursesByUserIdAsync(Guid userId)
    {
        var courseMemberEntities = await _context.CourseMembers
            .Where(cm => cm.MemberId == userId)
            .ToListAsync();

        var result = new List<Course>();
        
        foreach (var entity in courseMemberEntities)
        {
            var course = await GetByIdAsync(entity.CourseId);
            if (course is null)
                throw new InvalidOperationException(
                    $"Course with ID {entity.CourseId} was not found.");
            result.Add(course);
        }

        return result;
    }
}