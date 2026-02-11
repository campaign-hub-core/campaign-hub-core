using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Tests.Entities;

public class AdTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var ad = new Ad("adset-1", "Ad Creative 1", AdStatusEnum.Active);

        Assert.Equal("adset-1", ad.AdSetId);
        Assert.Equal("Ad Creative 1", ad.Name);
        Assert.Equal(AdStatusEnum.Active, ad.Status);
        Assert.Null(ad.ExternalId);
        Assert.NotNull(ad.Id);
    }

    [Fact]
    public void Pause_ShouldSetStatusPaused()
    {
        var ad = new Ad("adset-1", "Ad 1", AdStatusEnum.Active);

        ad.Pause();

        Assert.Equal(AdStatusEnum.Paused, ad.Status);
    }

    [Fact]
    public void Activate_ShouldSetStatusActive()
    {
        var ad = new Ad("adset-1", "Ad 1", AdStatusEnum.Paused);

        ad.Activate();

        Assert.Equal(AdStatusEnum.Active, ad.Status);
    }

    [Fact]
    public void SetExternalId_ShouldSetValue()
    {
        var ad = new Ad("adset-1", "Ad 1", AdStatusEnum.Active);

        ad.SetExternalId("ext-ad-1");

        Assert.Equal("ext-ad-1", ad.ExternalId);
    }
}
