using CampaignHub.Domain.Entities;

namespace CampaignHub.Domain.Tests.Entities;

public class OrganizationTests
{
    [Fact]
    public void Constructor_ShouldSetNameAndActiveTrue()
    {
        var org = new Organization("Test Org");

        Assert.Equal("Test Org", org.Name);
        Assert.True(org.Active);
        Assert.NotNull(org.Id);
    }

    [Fact]
    public void Update_ShouldChangeName()
    {
        var org = new Organization("Original");

        org.Update("Updated");

        Assert.Equal("Updated", org.Name);
    }

    [Fact]
    public void Disable_ShouldSetActiveFalse()
    {
        var org = new Organization("Test Org");

        org.Disable();

        Assert.False(org.Active);
    }
}
