using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Tests.Entities;

public class AdSetTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var adSet = new AdSet("camp-1", "Ad Set 1", AdSetStatusEnum.Active, 50.00m);

        Assert.Equal("camp-1", adSet.CampaignId);
        Assert.Equal("Ad Set 1", adSet.Name);
        Assert.Equal(AdSetStatusEnum.Active, adSet.Status);
        Assert.Equal(50.00m, adSet.DailyBudget);
        Assert.Null(adSet.ExternalId);
        Assert.Empty(adSet.Ads);
        Assert.NotNull(adSet.Id);
    }

    [Fact]
    public void Pause_ShouldSetStatusPaused()
    {
        var adSet = new AdSet("camp-1", "Ad Set 1", AdSetStatusEnum.Active, 50.00m);

        adSet.Pause();

        Assert.Equal(AdSetStatusEnum.Paused, adSet.Status);
    }

    [Fact]
    public void Activate_ShouldSetStatusActive()
    {
        var adSet = new AdSet("camp-1", "Ad Set 1", AdSetStatusEnum.Paused, 50.00m);

        adSet.Activate();

        Assert.Equal(AdSetStatusEnum.Active, adSet.Status);
    }

    [Fact]
    public void SetExternalId_ShouldSetValue()
    {
        var adSet = new AdSet("camp-1", "Ad Set 1", AdSetStatusEnum.Active, 50.00m);

        adSet.SetExternalId("ext-adset-1");

        Assert.Equal("ext-adset-1", adSet.ExternalId);
    }
}
