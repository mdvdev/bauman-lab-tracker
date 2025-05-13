using LabTracker.Domain.Entities;

namespace LabTracker.Application.Abstractions;

public interface ICourseRepository : ICrudRepository<Course, Guid>
{
}