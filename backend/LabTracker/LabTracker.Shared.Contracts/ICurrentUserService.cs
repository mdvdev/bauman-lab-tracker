using LabTracker.Users.Domain;

namespace LabTracker.Shared.Contracts;

public interface ICurrentUserService
{
    User User { get; }
}