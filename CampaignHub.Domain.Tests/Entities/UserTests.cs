using CampaignHub.Domain.Entities;

namespace CampaignHub.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_WithParameters_ShouldSetAllProperties()
    {
        var user = new User("John", "john@test.com", "hash123");

        Assert.Equal("John", user.Name);
        Assert.Equal("john@test.com", user.Email);
        Assert.Equal("hash123", user.PasswordHash);
        Assert.True(user.Active);
        Assert.NotNull(user.Id);
    }

    [Fact]
    public void Constructor_Parameterless_ShouldSetDefaults()
    {
        var user = new User();

        Assert.Equal(string.Empty, user.Name);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.False(user.Active);
    }

    [Fact]
    public void Update_ShouldChangeNameAndEmail()
    {
        var user = new User("John", "john@test.com", "hash123");

        user.Update("Jane", "jane@test.com");

        Assert.Equal("Jane", user.Name);
        Assert.Equal("jane@test.com", user.Email);
    }

    [Fact]
    public void Disable_ShouldSetActiveFalse()
    {
        var user = new User("John", "john@test.com", "hash123");

        user.Disable();

        Assert.False(user.Active);
    }
}
