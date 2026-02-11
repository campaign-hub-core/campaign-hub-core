using System.IdentityModel.Tokens.Jwt;
using CampaignHub.Application.Auth;
using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace CampaignHub.Application.Tests.Auth;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();

        var configData = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "SuperSecretKeyForTestingPurposesOnly1234567890!",
            ["Jwt:Issuer"] = "CampaignHub.Tests",
            ["Jwt:Audience"] = "CampaignHub.Tests",
            ["Jwt:ExpirationInMinutes"] = "30"
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _sut = new AuthService(_userRepository, _configuration);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new User("John", "john@test.com", passwordHash);
        _userRepository.GetByEmailAsync("john@test.com", Arg.Any<CancellationToken>())
            .Returns(user);
        var input = new LoginInput("john@test.com", "Password123");

        var result = await _sut.LoginAsync(input);

        Assert.NotNull(result);
        Assert.NotEmpty(result!.Token);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("John", result.UserName);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
    {
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        var input = new LoginInput("nonexistent@test.com", "Password123");

        var result = await _sut.LoginAsync(input);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserIsInactive()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new User("John", "john@test.com", passwordHash);
        user.Disable();
        _userRepository.GetByEmailAsync("john@test.com", Arg.Any<CancellationToken>())
            .Returns(user);
        var input = new LoginInput("john@test.com", "Password123");

        var result = await _sut.LoginAsync(input);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsWrong()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new User("John", "john@test.com", passwordHash);
        _userRepository.GetByEmailAsync("john@test.com", Arg.Any<CancellationToken>())
            .Returns(user);
        var input = new LoginInput("john@test.com", "WrongPassword");

        var result = await _sut.LoginAsync(input);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldNormalizeEmail()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new User("John", "john@test.com", passwordHash);
        _userRepository.GetByEmailAsync("john@test.com", Arg.Any<CancellationToken>())
            .Returns(user);
        var input = new LoginInput("  JOHN@Test.Com  ", "Password123");

        var result = await _sut.LoginAsync(input);

        Assert.NotNull(result);
        await _userRepository.Received(1).GetByEmailAsync("john@test.com", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LoginAsync_TokenShouldContainExpectedClaims()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new User("John", "john@test.com", passwordHash);
        _userRepository.GetByEmailAsync("john@test.com", Arg.Any<CancellationToken>())
            .Returns(user);
        var input = new LoginInput("john@test.com", "Password123");

        var result = await _sut.LoginAsync(input);

        Assert.NotNull(result);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result!.Token);

        Assert.Equal(user.Id, jwt.Subject);
        Assert.Equal("john@test.com", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal("John", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value);
        Assert.Equal("CampaignHub.Tests", jwt.Issuer);
        Assert.Contains("CampaignHub.Tests", jwt.Audiences);
    }
}
