using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LabTracker.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LabTracker.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    
    public TokenService(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config), "Configuration cannot be null.");
    }

    public (string, string) GenerateTokens(string username)
    {
        var key = Encoding.UTF8.GetBytes(
            _config["Jwt:Key"] ?? throw new ConfigurationErrorsException("Jwt:Key"));
        
        var accessTokenExpiration = TimeSpan.Parse(
            _config["Jwt:AccessTokenExpiration"] ?? throw new ConfigurationErrorsException("Jwt:AccessTokenExpiration"));
        
        var refreshTokenExpiration = TimeSpan.Parse(
            _config["Jwt:RefreshTokenExpiration"] ?? throw new ConfigurationErrorsException("Jwt:RefreshTokenExpiration"));
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Name, username) }.AsReadOnly()),
            Expires = DateTime.UtcNow.Add(accessTokenExpiration),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        }; 
        
        var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
        var accessTokenString = tokenHandler.WriteToken(accessToken);
        
        var refreshToken = Guid.NewGuid().ToString("N");

        // TODO: Add refreshToken to storage.
        
        return (accessTokenString, refreshToken);
    }

    public void RevokeRefreshToken(string username)
    {
        throw new NotImplementedException();
    }
}