using CampaignHub.Application.Users;
using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Interfaces;
using NSubstitute;

namespace CampaignHub.Application.Tests.Users;

public class UserServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _sut = new UserService(_userRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser_WhenEmailIsUnique()
    {
        var input = new CreateUserInput("John Doe", "john@test.com", "Password123");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _userRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<User>());

        var result = await _sut.CreateAsync(input);

        Assert.Equal("John Doe", result.Name);
        Assert.Equal("john@test.com", result.Email);
        Assert.True(result.Active);
        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldNormalizeEmail()
    {
        var input = new CreateUserInput("John", "  JOHN@Test.Com  ", "Password123");
        _userRepository.GetByEmailAsync("john@test.com", Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _userRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<User>());

        var result = await _sut.CreateAsync(input);

        Assert.Equal("john@test.com", result.Email);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        var input = new CreateUserInput("John", "john@test.com", "Password123");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new User("Existing", "john@test.com", "hash"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenExistsAndActive()
    {
        var user = new User("John", "john@test.com", "hash");
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _sut.GetByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _userRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _sut.GetByIdAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenInactive()
    {
        var user = new User("John", "john@test.com", "hash");
        user.Disable();
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _sut.GetByIdAsync(user.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnMatchingUsers()
    {
        var users = new List<User>
        {
            new("John", "john@test.com", "hash"),
            new("Johnny", "johnny@test.com", "hash")
        };
        _userRepository.GetByNameAsync("John", Arg.Any<CancellationToken>())
            .Returns(users);

        var result = await _sut.GetByNameAsync("John");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenExistsAndActive()
    {
        var user = new User("John", "john@test.com", "hash");
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);
        var input = new UpdateUserInput("Jane", "jane@test.com");

        var result = await _sut.UpdateAsync(user.Id, input);

        Assert.NotNull(result);
        Assert.Equal("Jane", result!.Name);
        Assert.Equal("jane@test.com", result.Email);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotFound()
    {
        _userRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _sut.UpdateAsync("nonexistent", new UpdateUserInput("Name", "email@test.com"));

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDisableUser_WhenExistsAndActive()
    {
        var user = new User("John", "john@test.com", "hash");
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _sut.DeleteAsync(user.Id);

        Assert.True(result);
        Assert.False(user.Active);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        _userRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _sut.DeleteAsync("nonexistent");

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenAlreadyInactive()
    {
        var user = new User("John", "john@test.com", "hash");
        user.Disable();
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _sut.DeleteAsync(user.Id);

        Assert.False(result);
    }
}
