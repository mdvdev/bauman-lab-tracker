using LabTracker.Domain.Entities;

namespace LabTracker.Application.Abstractions;

public interface IUserRefreshTokenRepository
{
    Task<UserRefreshToken?> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserRefreshToken userRefreshToken);
    Task DeleteAsync(Guid tokenId);
}