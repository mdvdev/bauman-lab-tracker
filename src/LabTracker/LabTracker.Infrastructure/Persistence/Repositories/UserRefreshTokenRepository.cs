using LabTracker.Application.Abstractions;
using LabTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class UserRefreshTokenRepository : IUserRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRefreshTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<UserRefreshToken?> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.UserRefreshTokens.
            FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task AddAsync(UserRefreshToken userRefreshToken)
    {
        await _dbContext.UserRefreshTokens.AddAsync(userRefreshToken);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid tokenId)
    {
        var token = await _dbContext.UserRefreshTokens
            .FirstOrDefaultAsync(t => t.Id == tokenId);
        
        _dbContext.UserRefreshTokens.FindAsync(token);
    }
}