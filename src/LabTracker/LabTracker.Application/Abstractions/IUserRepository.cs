using LabTracker.Domain.Entities;

namespace LabTracker.Application.Abstractions;

public interface IUserRepository : ICrudRepository<User, Guid>
{
}