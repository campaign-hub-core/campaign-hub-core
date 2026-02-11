using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Tests.Entities;

public class AdAccountTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Increase sales", AdPlatformEnum.MetaAds);

        Assert.Equal("cust-1", adAccount.CustomerId);
        Assert.Equal(5000m, adAccount.MonthlyBudget);
        Assert.Equal("Increase sales", adAccount.Goal);
        Assert.Equal(AdPlatformEnum.MetaAds, adAccount.AdPlatform);
        Assert.Null(adAccount.ExternalId);
        Assert.Null(adAccount.LastSyncedAt);
        Assert.Empty(adAccount.Campaigns);
        Assert.NotNull(adAccount.Id);
    }

    [Fact]
    public void Update_ShouldChangeBudgetAndGoal()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Increase sales", AdPlatformEnum.MetaAds);

        adAccount.Update(10000m, "New goal");

        Assert.Equal(10000m, adAccount.MonthlyBudget);
        Assert.Equal("New goal", adAccount.Goal);
    }

    [Fact]
    public void UpdateMonthlyBudget_ShouldChangeBudget()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.GoogleAds);

        adAccount.UpdateMonthlyBudget(7500m);

        Assert.Equal(7500m, adAccount.MonthlyBudget);
    }

    [Fact]
    public void SetExternalId_ShouldSetValue()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);

        adAccount.SetExternalId("act_123456");

        Assert.Equal("act_123456", adAccount.ExternalId);
    }

    [Fact]
    public void MarkSynced_ShouldSetLastSyncedAt()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        var before = DateTime.UtcNow;

        adAccount.MarkSynced();

        Assert.NotNull(adAccount.LastSyncedAt);
        Assert.InRange(adAccount.LastSyncedAt.Value, before, DateTime.UtcNow);
    }
}
