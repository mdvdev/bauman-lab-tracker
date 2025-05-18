using LabTracker.Shared.Contracts;

namespace LabTracker.User.Abstractions.Repositories;

public interface IUserRepository : ICrudRepository<Users.Domain.User, Guid>
{
}