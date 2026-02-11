using CampaignHub.Application.DTOs.Meta;
using CampaignHub.Application.Exceptions;
using CampaignHub.Application.Interfaces;
using CampaignHub.Application.Services;
using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Domain.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace CampaignHub.Application.Tests.Services;

public class MetaAdsSyncServiceTests
{
    private readonly IAdAccountRepository _adAccountRepo;
    private readonly ICampaignRepository _campaignRepo;
    private readonly IAdSetRepository _adSetRepo;
    private readonly IAdRepository _adRepo;
    private readonly IMetricCampaignRepository _metricRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMetaAdsApiClient _metaClient;
    private readonly MetaAdsSyncService _sut;

    public MetaAdsSyncServiceTests()
    {
        _adAccountRepo = Substitute.For<IAdAccountRepository>();
        _campaignRepo = Substitute.For<ICampaignRepository>();
        _adSetRepo = Substitute.For<IAdSetRepository>();
        _adRepo = Substitute.For<IAdRepository>();
        _metricRepo = Substitute.For<IMetricCampaignRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _metaClient = Substitute.For<IMetaAdsApiClient>();
        _sut = new MetaAdsSyncService(_adAccountRepo, _campaignRepo, _adSetRepo, _adRepo, _metricRepo, _unitOfWork, _metaClient);
    }

    [Fact]
    public async Task SyncAdAccountAsync_ShouldThrow_WhenAdAccountNotFound()
    {
        _adAccountRepo.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((AdAccount?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.SyncAdAccountAsync("nonexistent"));
    }

    [Fact]
    public async Task SyncAdAccountAsync_ShouldThrow_WhenNoExternalId()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        _adAccountRepo.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.SyncAdAccountAsync(adAccount.Id));
    }

    [Fact]
    public async Task SyncAdAccountAsync_ShouldThrow_WhenPlatformIsNotMetaAds()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.GoogleAds);
        adAccount.SetExternalId("act_123");
        _adAccountRepo.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.SyncAdAccountAsync(adAccount.Id));
    }

    [Fact]
    public async Task SyncAdAccountAsync_ShouldSyncCampaignsAndMarkSynced()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        adAccount.SetExternalId("act_123");
        _adAccountRepo.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        var metaCampaigns = new List<MetaCampaignDto>
        {
            new("mc-1", "Campaign 1", "ACTIVE", "2026-01-01", "2026-12-31")
        };
        _metaClient.GetCampaignsAsync("act_123", Arg.Any<CancellationToken>())
            .Returns(metaCampaigns);
        _metaClient.GetAdSetsAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns(new List<MetaAdSetDto>());
        _metaClient.GetCampaignInsightsAsync("act_123", Arg.Any<DateOnly>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(new List<MetaInsightDto>());
        _campaignRepo.GetByExternalIdAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns((Campaign?)null);
        _campaignRepo.AddAsync(Arg.Any<Campaign>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Campaign>());

        var result = await _sut.SyncAdAccountAsync(adAccount.Id);

        Assert.Equal(1, result.CampaignsSynced);
        Assert.Equal(0, result.AdSetsSynced);
        Assert.Equal(0, result.AdsSynced);
        Assert.NotNull(adAccount.LastSyncedAt);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SyncAdAccountAsync_ShouldUpdateExistingCampaign()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        adAccount.SetExternalId("act_123");
        _adAccountRepo.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        var existingCampaign = new Campaign(adAccount.Id, "Old Name", DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
        existingCampaign.SetExternalId("mc-1");

        var metaCampaigns = new List<MetaCampaignDto>
        {
            new("mc-1", "Updated Name", "PAUSED", "2026-01-01", "2026-12-31")
        };
        _metaClient.GetCampaignsAsync("act_123", Arg.Any<CancellationToken>())
            .Returns(metaCampaigns);
        _metaClient.GetAdSetsAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns(new List<MetaAdSetDto>());
        _metaClient.GetCampaignInsightsAsync("act_123", Arg.Any<DateOnly>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(new List<MetaInsightDto>());
        _campaignRepo.GetByExternalIdAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns(existingCampaign);

        var result = await _sut.SyncAdAccountAsync(adAccount.Id);

        Assert.Equal(1, result.CampaignsSynced);
        Assert.Equal("Updated Name", existingCampaign.Name);
        Assert.Equal(CampaignStatusEnum.Paused, existingCampaign.CampaignStatus);
    }

    [Fact]
    public async Task SyncAdAccountAsync_ShouldSyncAdSetsAndAds()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        adAccount.SetExternalId("act_123");
        _adAccountRepo.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        _metaClient.GetCampaignsAsync("act_123", Arg.Any<CancellationToken>())
            .Returns(new List<MetaCampaignDto> { new("mc-1", "Camp", "ACTIVE", null, null) });
        _metaClient.GetAdSetsAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns(new List<MetaAdSetDto> { new("as-1", "mc-1", "AdSet 1", "ACTIVE", "5000") });
        _metaClient.GetAdsAsync("as-1", Arg.Any<CancellationToken>())
            .Returns(new List<MetaAdDto> { new("ad-1", "as-1", "Ad 1", "ACTIVE") });
        _metaClient.GetCampaignInsightsAsync("act_123", Arg.Any<DateOnly>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(new List<MetaInsightDto>());

        _campaignRepo.GetByExternalIdAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns((Campaign?)null);
        _campaignRepo.AddAsync(Arg.Any<Campaign>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Campaign>());
        _adSetRepo.GetByExternalIdAsync("as-1", Arg.Any<CancellationToken>())
            .Returns((AdSet?)null);
        _adSetRepo.AddAsync(Arg.Any<AdSet>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<AdSet>());
        _adRepo.GetByExternalIdAsync("ad-1", Arg.Any<CancellationToken>())
            .Returns((Ad?)null);
        _adRepo.AddAsync(Arg.Any<Ad>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Ad>());

        var result = await _sut.SyncAdAccountAsync(adAccount.Id);

        Assert.Equal(1, result.CampaignsSynced);
        Assert.Equal(1, result.AdSetsSynced);
        Assert.Equal(1, result.AdsSynced);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task SyncAdAccountAsync_ShouldSyncInsights()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        adAccount.SetExternalId("act_123");
        _adAccountRepo.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        _metaClient.GetCampaignsAsync("act_123", Arg.Any<CancellationToken>())
            .Returns(new List<MetaCampaignDto>());

        var campaign = new Campaign(adAccount.Id, "Campaign", DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
        campaign.SetExternalId("mc-1");

        var insights = new List<MetaInsightDto>
        {
            new("mc-1", "2026-01-01", "2026-01-31", "150000", 25, 5, "50000")
        };
        _metaClient.GetCampaignInsightsAsync("act_123", Arg.Any<DateOnly>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(insights);
        _campaignRepo.GetByExternalIdAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns(campaign);
        _metricRepo.GetByCampaignAndPeriodAsync(campaign.Id, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((MetricCampaign?)null);
        _metricRepo.AddAsync(Arg.Any<MetricCampaign>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<MetricCampaign>());

        var result = await _sut.SyncAdAccountAsync(adAccount.Id);

        Assert.Equal(1, result.MetricsSynced);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task SyncAdAccountAsync_ShouldCaptureErrors_WhenCampaignSyncFails()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        adAccount.SetExternalId("act_123");
        _adAccountRepo.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        _metaClient.GetCampaignsAsync("act_123", Arg.Any<CancellationToken>())
            .Returns(new List<MetaCampaignDto> { new("mc-1", "Camp", "ACTIVE", null, null) });
        _metaClient.GetAdSetsAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns(new List<MetaAdSetDto>());
        _metaClient.GetCampaignInsightsAsync("act_123", Arg.Any<DateOnly>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(new List<MetaInsightDto>());

        _campaignRepo.GetByExternalIdAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns((Campaign?)null);
        _campaignRepo.AddAsync(Arg.Any<Campaign>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("DB Error"));

        var result = await _sut.SyncAdAccountAsync(adAccount.Id);

        Assert.Equal(0, result.CampaignsSynced);
        Assert.Single(result.Errors);
        Assert.Contains("mc-1", result.Errors[0]);
    }

    [Fact]
    public async Task SyncAdAccountAsync_ShouldUpdateExistingAdSet()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        adAccount.SetExternalId("act_123");
        _adAccountRepo.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        _metaClient.GetCampaignsAsync("act_123", Arg.Any<CancellationToken>())
            .Returns(new List<MetaCampaignDto> { new("mc-1", "Camp", "ACTIVE", null, null) });

        var existingCampaign = new Campaign(adAccount.Id, "Camp", DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
        _campaignRepo.GetByExternalIdAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns(existingCampaign);

        var existingAdSet = new AdSet(existingCampaign.Id, "Old Name", AdSetStatusEnum.Active, 30m);
        _adSetRepo.GetByExternalIdAsync("as-1", Arg.Any<CancellationToken>())
            .Returns(existingAdSet);

        _metaClient.GetAdSetsAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns(new List<MetaAdSetDto> { new("as-1", "mc-1", "AdSet Updated", "PAUSED", "5000") });
        _metaClient.GetAdsAsync("as-1", Arg.Any<CancellationToken>())
            .Returns(new List<MetaAdDto>());
        _metaClient.GetCampaignInsightsAsync("act_123", Arg.Any<DateOnly>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(new List<MetaInsightDto>());

        var result = await _sut.SyncAdAccountAsync(adAccount.Id);

        Assert.Equal(1, result.AdSetsSynced);
        Assert.Equal(AdSetStatusEnum.Paused, existingAdSet.Status);
    }

    [Fact]
    public async Task SyncAdAccountAsync_ShouldUpdateExistingAd()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        adAccount.SetExternalId("act_123");
        _adAccountRepo.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        _metaClient.GetCampaignsAsync("act_123", Arg.Any<CancellationToken>())
            .Returns(new List<MetaCampaignDto> { new("mc-1", "Camp", "ACTIVE", null, null) });

        var existingCampaign = new Campaign(adAccount.Id, "Camp", DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
        _campaignRepo.GetByExternalIdAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns(existingCampaign);

        _adSetRepo.GetByExternalIdAsync("as-1", Arg.Any<CancellationToken>())
            .Returns((AdSet?)null);
        _adSetRepo.AddAsync(Arg.Any<AdSet>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<AdSet>());

        var existingAd = new Ad("as-1", "Old Ad", AdStatusEnum.Active);
        _adRepo.GetByExternalIdAsync("ad-1", Arg.Any<CancellationToken>())
            .Returns(existingAd);

        _metaClient.GetAdSetsAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns(new List<MetaAdSetDto> { new("as-1", "mc-1", "AdSet", "ACTIVE", "5000") });
        _metaClient.GetAdsAsync("as-1", Arg.Any<CancellationToken>())
            .Returns(new List<MetaAdDto> { new("ad-1", "as-1", "Ad Updated", "PAUSED") });
        _metaClient.GetCampaignInsightsAsync("act_123", Arg.Any<DateOnly>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(new List<MetaInsightDto>());

        var result = await _sut.SyncAdAccountAsync(adAccount.Id);

        Assert.Equal(1, result.AdsSynced);
        Assert.Equal(AdStatusEnum.Paused, existingAd.Status);
    }

    [Fact]
    public async Task SyncAdAccountAsync_ShouldUpdateExistingMetric()
    {
        var adAccount = new AdAccount("cust-1", 5000m, "Goal", AdPlatformEnum.MetaAds);
        adAccount.SetExternalId("act_123");
        _adAccountRepo.GetByIdAsync(adAccount.Id, Arg.Any<CancellationToken>())
            .Returns(adAccount);

        _metaClient.GetCampaignsAsync("act_123", Arg.Any<CancellationToken>())
            .Returns(new List<MetaCampaignDto>());

        var campaign = new Campaign(adAccount.Id, "Camp", DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
        campaign.SetExternalId("mc-1");
        _campaignRepo.GetByExternalIdAsync("mc-1", Arg.Any<CancellationToken>())
            .Returns(campaign);

        var existingMetric = new MetricCampaign(campaign.Id, new DateTime(2026, 1, 1), 100m, 5, null, null);
        _metricRepo.GetByCampaignAndPeriodAsync(campaign.Id, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(existingMetric);

        var insights = new List<MetaInsightDto>
        {
            new("mc-1", "2026-01-01", "2026-01-31", "200000", 10, 3, "30000")
        };
        _metaClient.GetCampaignInsightsAsync("act_123", Arg.Any<DateOnly>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(insights);

        var result = await _sut.SyncAdAccountAsync(adAccount.Id);

        Assert.Equal(1, result.MetricsSynced);
        Assert.Equal(2000m, existingMetric.Expenses); // 200000 / 100
        Assert.Equal(10, existingMetric.Leads);
    }
}
