using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Tests.Entities;

public class CampaignTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var start = new DateTime(2026, 1, 1);
        var end = new DateTime(2026, 12, 31);

        var campaign = new Campaign("acc-1", "Summer Sale", start, end);

        Assert.Equal("acc-1", campaign.AdAccountId);
        Assert.Equal("Summer Sale", campaign.Name);
        Assert.Equal(start, campaign.StartDate);
        Assert.Equal(end, campaign.EndDate);
        Assert.Equal(CampaignStatusEnum.Active, campaign.CampaignStatus);
        Assert.Null(campaign.ExternalId);
        Assert.Empty(campaign.Metrics);
        Assert.Empty(campaign.AdSets);
        Assert.NotNull(campaign.Id);
    }

    [Fact]
    public void Update_ShouldChangeNameAndDates()
    {
        var campaign = new Campaign("acc-1", "Old Name", DateTime.Now, DateTime.Now.AddDays(30));
        var newStart = new DateTime(2026, 6, 1);
        var newEnd = new DateTime(2026, 6, 30);

        campaign.Update("New Name", newStart, newEnd);

        Assert.Equal("New Name", campaign.Name);
        Assert.Equal(newStart, campaign.StartDate);
        Assert.Equal(newEnd, campaign.EndDate);
    }

    [Fact]
    public void Pause_ShouldSetStatusPaused()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));

        campaign.Pause();

        Assert.Equal(CampaignStatusEnum.Paused, campaign.CampaignStatus);
    }

    [Fact]
    public void Activate_ShouldSetStatusActive()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));
        campaign.Pause();

        campaign.Activate();

        Assert.Equal(CampaignStatusEnum.Active, campaign.CampaignStatus);
    }

    [Fact]
    public void Complete_ShouldSetStatusCompleted()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));

        campaign.Complete();

        Assert.Equal(CampaignStatusEnum.Completed, campaign.CampaignStatus);
    }

    [Fact]
    public void SetExternalId_ShouldSetValue()
    {
        var campaign = new Campaign("acc-1", "Campaign", DateTime.Now, DateTime.Now.AddDays(30));

        campaign.SetExternalId("ext-123");

        Assert.Equal("ext-123", campaign.ExternalId);
    }
}
