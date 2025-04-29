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
    private readonly IUserRepository _userRepository;
    
    public TokenService(IConfiguration config, IUserRepository userRepository)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config), "Configuration cannot be null.");
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository), "UserRepository cannot be null.");
    }

    public (string, string) GenerateTokens(string username)
    {
        var accessToken = GenerateAccessToken();
        var refreshToken = GenerateRefreshToken();

        // TODO: Add refreshToken to storage.
        
        return (accessToken, refreshToken);
    }

    public string RefreshAccessToken(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public void RevokeRefreshToken(string username)
    {
        throw new NotImplementedException();
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    private string GenerateAccessToken()
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
        
        return accessTokenString;
    }
}