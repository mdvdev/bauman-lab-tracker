using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LabTracker.Application.Abstractions;
using LabTracker.Domain.ValueObjects;
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

    public (string, string) GenerateTokens(Guid userId, Role role)
    {
        var accessToken = GenerateAccessToken(userId, role);
        var refreshToken = GenerateRefreshToken();

        return (accessToken, refreshToken);
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    public string GenerateAccessToken(Guid userId, Role role)
    {
        var key = Encoding.UTF8.GetBytes(
            _config["Jwt:Key"] ?? throw new ConfigurationErrorsException("Jwt:Key"));

        var accessTokenExpiration = TimeSpan.Parse(
            _config["Jwt:AccessTokenExpiration"] ??
            throw new ConfigurationErrorsException("Jwt:AccessTokenExpiration"));

        var tokenHandler = new JwtSecurityTokenHandler();
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, role.ToString())
        };

        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(accessTokenExpiration),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(accessTokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}