using LabTracker.Courses.Domain;
using LabTracker.Shared.Contracts;

namespace LabTracker.Courses.Abstractions.Repositories;

public interface ICourseRepository : ICrudRepository<Course, Guid>
{
    Task<IEnumerable<Course>> GetCoursesByUserIdAsync(Guid userId);
}