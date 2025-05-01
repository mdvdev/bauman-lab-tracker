using System;
using System.Collections.Generic;
using System.Configuration;
using FluentAssertions;
using JetBrains.Annotations;
using LabTracker.Application.Abstractions;
using LabTracker.Domain.ValueObjects;
using LabTracker.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;
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

        Guid userId = Guid.NewGuid();
        Role role = Role.Student;
        var service = new TokenService(config);

        var (accessToken, refreshToken) = service.GenerateTokens(userId, role);

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

        Guid userId = Guid.NewGuid();
        Role role = Role.Student;
        var service = new TokenService(config);

        var exception = Assert.Throws<ConfigurationErrorsException>(() => service.GenerateTokens(userId, role));

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

        Guid userId = Guid.NewGuid();
        Role role = Role.Student;
        var service = new TokenService(config);

        var exception = Assert.Throws<ConfigurationErrorsException>(() => service.GenerateTokens(userId, role));

        Assert.Contains("Jwt:AccessTokenExpiration", exception.Message);
    }

    [Fact]
    public void GenerateToken_MissingRefreshTokenExpiration_ShouldReturnTokens()
    {
        var config = CreateTestConfiguration(new Dictionary<string, string>
        {
            { "Jwt:Key", "MySuperSecureKeyThatIsLongEnough1234567890" },
            { "Jwt:AccessTokenExpiration", "00:15:00" },
        });

        Guid userId = Guid.NewGuid();
        Role role = Role.Student;
        var service = new TokenService(config);

        var (accessToken, refreshToken) = service.GenerateTokens(userId, role);

        accessToken.Should().NotBeNullOrEmpty();
        refreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateTokens_NullConfiguration_ShouldThrowArgumentNullException()
    {
        IConfiguration config = null;

        Assert.Throws<ArgumentNullException>(() => new TokenService(config));
    }

    private IConfiguration CreateTestConfiguration(Dictionary<string, string> settings)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }
}