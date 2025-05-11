using LabTracker.Domain.Entities;

namespace LabTracker.Application.Contracts;

public interface IUserRepository : ICrudRepository<User, Guid>
{
}