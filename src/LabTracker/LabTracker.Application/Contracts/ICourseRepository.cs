using LabTracker.Domain.Entities;

namespace LabTracker.Application.Contracts;

public interface ICourseRepository : ICrudRepository<Course, Guid>
{
}