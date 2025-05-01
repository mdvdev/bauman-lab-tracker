using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Abstractions;

public interface ITokenService
{
    (string, string) GenerateTokens(Guid userId, Role role);
    string GenerateAccessToken(Guid userId, Role role);
    string GenerateRefreshToken();
}