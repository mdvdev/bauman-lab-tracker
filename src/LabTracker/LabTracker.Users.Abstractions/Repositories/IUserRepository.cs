using LabTracker.Shared.Contracts;
using LabTracker.Users.Domain;

namespace LabTracker.User.Abstractions.Repositories;

public interface IUserRepository : ICrudRepository<Users.Domain.User, Guid>
{
    Task AddRoleToUserAsync(Guid userId, Role role);
    Task RemoveRoleFromUserAsync(Guid userId, Role role);
}