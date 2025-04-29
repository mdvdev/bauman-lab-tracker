namespace LabTracker.Application.Abstractions;

public interface ITokenService
{
    (string, string) GenerateTokens(string username);
    
    string RefreshAccessToken(string refreshToken);
    
    void RevokeRefreshToken(string username);
}