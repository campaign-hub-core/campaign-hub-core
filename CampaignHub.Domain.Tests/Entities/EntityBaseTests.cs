using CampaignHub.Domain.Entities;

namespace CampaignHub.Domain.Tests.Entities;

public class EntityBaseTests
{
    [Fact]
    public void Constructor_ShouldGenerateUniqueId()
    {
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        Assert.NotNull(entity1.Id);
        Assert.NotNull(entity2.Id);
        Assert.NotEqual(entity1.Id, entity2.Id);
    }

    [Fact]
    public void Constructor_ShouldSetIdAsValidGuid()
    {
        var entity = new TestEntity();

        Assert.True(Guid.TryParse(entity.Id, out _));
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAtToUtcNow()
    {
        var before = DateTime.UtcNow;
        var entity = new TestEntity();
        var after = DateTime.UtcNow;

        Assert.InRange(entity.CreatedAt, before, after);
    }

    private class TestEntity : Entity { }
}
