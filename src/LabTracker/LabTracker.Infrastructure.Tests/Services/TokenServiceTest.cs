using System;
using System.Collections.Generic;
using System.Configuration;
using FluentAssertions;
using JetBrains.Annotations;
using LabTracker.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LabTracker.Infrastructure.Tests.Services;

[TestSubject(typeof(TokenService))]
public class TokenServiceTest
{
    [Fact]
    public void GenerateTokens_ShouldReturnTokens()
    {
        var config = CreateTestConfiguration(new Dictionary<string, string>
        {
            { "Jwt:Key", "MySuperSecureKeyThatIsLongEnough1234567890" },
            { "Jwt:AccessTokenExpiration", "00:15:00" },
            { "Jwt:RefreshTokenExpiration", "00:30:00" }
        });
        
        const string username = "user123";
        var service = new TokenService(config);

        var (accessToken, refreshToken) = service.GenerateTokens(username);

        accessToken.Should().NotBeNullOrEmpty();
        refreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateToken_MissingJwtKey_ShouldThrowConfigurationErrorsException()
    {
        var config = CreateTestConfiguration(new Dictionary<string, string>
        {
            { "Jwt:AccessTokenExpiration", "00:15:00" },
            { "Jwt:RefreshTokenExpiration", "00:30:00" }
        });        
        
        const string username = "user123";
        var service = new TokenService(config);
        
        var exception = Assert.Throws<ConfigurationErrorsException>(() => service.GenerateTokens(username));

        Assert.Contains("Jwt:Key", exception.Message);
    }

    [Fact]
    public void GenerateToken_MissingJwtAccessExpiration_ShouldThrowConfigurationErrorsException()
    {
        var config = CreateTestConfiguration(new Dictionary<string, string>
        {
            { "Jwt:Key", "MySuperSecureKeyThatIsLongEnough1234567890" },
            { "Jwt:RefreshTokenExpiration", "00:30:00" }
        });
        
        const string username = "user123";
        var service = new TokenService(config);

        var exception = Assert.Throws<ConfigurationErrorsException>(() => service.GenerateTokens(username));
        
        Assert.Contains("Jwt:AccessTokenExpiration", exception.Message);
    }

    [Fact]
    public void GenerateToken_MissingRefreshTokenExpiration_ShouldThrowConfigurationErrorsException()
    {
        var config = CreateTestConfiguration(new Dictionary<string, string>
        {
            { "Jwt:Key", "MySuperSecureKeyThatIsLongEnough1234567890" },
            { "Jwt:AccessTokenExpiration", "00:15:00" },
        });
        
        const string username = "user123";
        var service = new TokenService(config);

        var exception = Assert.Throws<ConfigurationErrorsException>(() => service.GenerateTokens(username));
        
        Assert.Contains("Jwt:RefreshTokenExpiration", exception.Message);
    }

    [Fact]
    public void GenerateTokens_NullConfiguration_ShouldThrowArgumentNullException()
    {
        IConfiguration config = null;
        
        Assert.Throws<ArgumentNullException>(() => new TokenService(config));
    }

    [Fact]
    public void GenerateTokens_NullUsername_ShouldThrowArgumentNullException()
    {
        var config = CreateTestConfiguration(new Dictionary<string, string>
        {
            { "Jwt:Key", "MySuperSecureKeyThatIsLongEnough1234567890" },
            { "Jwt:AccessTokenExpiration", "00:15:00" },
            { "Jwt:RefreshTokenExpiration", "00:30:00" }
        });
        
        const string username = null;
        var service = new TokenService(config);

        Assert.Throws<ArgumentNullException>(() => service.GenerateTokens(username));
    }

    [Fact]
    public void GenerateTokens_UserNotExistInRepository_ShouldThrowConfigurationErrorsException()
    {
        
    }
    
    private IConfiguration CreateTestConfiguration(Dictionary<string, string> settings)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }
}
    