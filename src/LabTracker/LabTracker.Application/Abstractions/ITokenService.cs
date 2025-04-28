namespace LabTracker.Application.Abstractions;

public interface ITokenService
{
    (string, string) GenerateTokens(string username);

    void RevokeRefreshToken(string username);
}