using CampaignHub.Application.DTOs;
using CampaignHub.Application.DTOs.Meta;
using CampaignHub.Application.Exceptions;
using CampaignHub.Application.Interfaces;
using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Domain.Interfaces;

namespace CampaignHub.Application.Services;

public class MetaAdsSyncService
{
    private readonly IAdAccountRepository _adAccountRepo;
    private readonly ICampaignRepository _campaignRepo;
    private readonly IAdSetRepository _adSetRepo;
    private readonly IAdRepository _adRepo;
    private readonly IMetricCampaignRepository _metricRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMetaAdsApiClient _metaClient;

    public MetaAdsSyncService(
        IAdAccountRepository adAccountRepo,
        ICampaignRepository campaignRepo,
        IAdSetRepository adSetRepo,
        IAdRepository adRepo,
        IMetricCampaignRepository metricRepo,
        IUnitOfWork unitOfWork,
        IMetaAdsApiClient metaClient)
    {
        _adAccountRepo = adAccountRepo;
        _campaignRepo = campaignRepo;
        _adSetRepo = adSetRepo;
        _adRepo = adRepo;
        _metricRepo = metricRepo;
        _unitOfWork = unitOfWork;
        _metaClient = metaClient;
    }

    public async Task<SyncResultDto> SyncAdAccountAsync(string adAccountId, CancellationToken ct = default)
    {
        var adAccount = await _adAccountRepo.GetByIdAsync(adAccountId, ct);
        if (adAccount is null)
            throw new EntityNotFoundException(nameof(AdAccount), adAccountId);

        if (string.IsNullOrWhiteSpace(adAccount.ExternalId))
            throw new InvalidOperationException("AdAccount does not have an ExternalId configured for Meta Ads sync.");

        if (adAccount.AdPlatform != AdPlatformEnum.MetaAds)
            throw new InvalidOperationException($"AdAccount platform is {adAccount.AdPlatform}, expected MetaAds.");

        var errors = new List<string>();
        int campaignsSynced = 0;
        int adSetsSynced = 0;
        int adsSynced = 0;
        int metricsSynced = 0;

        // 1. Sync campaigns
        var metaCampaigns = await _metaClient.GetCampaignsAsync(adAccount.ExternalId, ct);
        foreach (var mc in metaCampaigns)
        {
            try
            {
                var campaign = await UpsertCampaignAsync(adAccount.Id, mc, ct);
                campaignsSynced++;

                // 2. Sync ad sets for each campaign
                var metaAdSets = await _metaClient.GetAdSetsAsync(mc.Id, ct);
                foreach (var mas in metaAdSets)
                {
                    try
                    {
                        await UpsertAdSetAsync(campaign.Id, mas, ct);
                        adSetsSynced++;

                        // 3. Sync ads for each ad set
                        var metaAds = await _metaClient.GetAdsAsync(mas.Id, ct);
                        foreach (var ma in metaAds)
                        {
                            try
                            {
                                await UpsertAdAsync(ma, ct);
                                adsSynced++;
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Error syncing ad {ma.Id}: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error syncing ad set {mas.Id}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error syncing campaign {mc.Id}: {ex.Message}");
            }
        }

        // 4. Sync insights (last 30 days)
        try
        {
            var until = DateOnly.FromDateTime(DateTime.UtcNow);
            var since = until.AddDays(-30);
            var insights = await _metaClient.GetCampaignInsightsAsync(adAccount.ExternalId, since, until, ct);
            foreach (var insight in insights)
            {
                try
                {
                    await UpsertMetricAsync(insight, ct);
                    metricsSynced++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Error syncing metric for campaign {insight.CampaignId}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add($"Error fetching insights: {ex.Message}");
        }

        // 5. Mark synced and save
        adAccount.MarkSynced();
        await _unitOfWork.SaveChangesAsync(ct);

        return new SyncResultDto(
            adAccount.Id,
            campaignsSynced,
            adSetsSynced,
            adsSynced,
            metricsSynced,
            adAccount.LastSyncedAt!.Value,
            errors
        );
    }

    private async Task<Campaign> UpsertCampaignAsync(string adAccountId, MetaCampaignDto dto, CancellationToken ct)
    {
        var existing = await _campaignRepo.GetByExternalIdAsync(dto.Id, ct);
        if (existing is not null)
        {
            existing.Update(
                dto.Name,
                ParseDateTimeOrDefault(dto.StartTime, existing.StartDate),
                ParseDateTimeOrDefault(dto.StopTime, existing.EndDate)
            );
            existing.SetExternalId(dto.Id);
            ApplyCampaignStatus(existing, dto.Status);
            return existing;
        }

        var campaign = new Campaign(
            adAccountId,
            dto.Name,
            ParseDateTimeOrDefault(dto.StartTime, DateTime.UtcNow),
            ParseDateTimeOrDefault(dto.StopTime, DateTime.UtcNow.AddYears(1))
        );
        campaign.SetExternalId(dto.Id);
        ApplyCampaignStatus(campaign, dto.Status);
        await _campaignRepo.AddAsync(campaign, ct);
        return campaign;
    }

    private async Task UpsertAdSetAsync(string campaignId, MetaAdSetDto dto, CancellationToken ct)
    {
        var existing = await _adSetRepo.GetByExternalIdAsync(dto.Id, ct);
        if (existing is not null)
        {
            existing.SetExternalId(dto.Id);
            existing.Status = ParseAdSetStatus(dto.Status);
            existing.DailyBudget = ParseDecimalOrDefault(dto.DailyBudget);
            return;
        }

        var adSet = new AdSet(campaignId, dto.Name, ParseAdSetStatus(dto.Status), ParseDecimalOrDefault(dto.DailyBudget));
        adSet.SetExternalId(dto.Id);
        await _adSetRepo.AddAsync(adSet, ct);
    }

    private async Task UpsertAdAsync(MetaAdDto dto, CancellationToken ct)
    {
        var existing = await _adRepo.GetByExternalIdAsync(dto.Id, ct);
        if (existing is not null)
        {
            existing.SetExternalId(dto.Id);
            existing.Status = ParseAdStatus(dto.Status);
            return;
        }

        var ad = new Ad(dto.AdSetId, dto.Name, ParseAdStatus(dto.Status));
        ad.SetExternalId(dto.Id);
        await _adRepo.AddAsync(ad, ct);
    }

    private async Task UpsertMetricAsync(MetaInsightDto dto, CancellationToken ct)
    {
        var campaign = await _campaignRepo.GetByExternalIdAsync(dto.CampaignId, ct);
        if (campaign is null) return;

        var referencePeriod = DateTime.Parse(dto.DateStart);
        var existing = await _metricRepo.GetByCampaignAndPeriodAsync(campaign.Id, referencePeriod, ct);

        var expenses = ParseDecimalOrDefault(dto.Spend);
        var leads = dto.Leads;
        var sales = dto.Purchases > 0 ? dto.Purchases.ToString() : null;
        var revenue = dto.PurchaseValue;

        if (existing is not null)
        {
            existing.Update(expenses, leads, sales, revenue);
            return;
        }

        var metric = new MetricCampaign(campaign.Id, referencePeriod, expenses, leads, sales, revenue);
        await _metricRepo.AddAsync(metric, ct);
    }

    private static void ApplyCampaignStatus(Campaign campaign, string metaStatus)
    {
        switch (metaStatus.ToUpperInvariant())
        {
            case "ACTIVE":
                campaign.Activate();
                break;
            case "PAUSED":
                campaign.Pause();
                break;
            default:
                campaign.Pause();
                break;
        }
    }

    private static AdSetStatusEnum ParseAdSetStatus(string status) =>
        status.ToUpperInvariant() switch
        {
            "ACTIVE" => AdSetStatusEnum.Active,
            "PAUSED" => AdSetStatusEnum.Paused,
            "DELETED" => AdSetStatusEnum.Deleted,
            "ARCHIVED" => AdSetStatusEnum.Archived,
            _ => AdSetStatusEnum.Paused
        };

    private static AdStatusEnum ParseAdStatus(string status) =>
        status.ToUpperInvariant() switch
        {
            "ACTIVE" => AdStatusEnum.Active,
            "PAUSED" => AdStatusEnum.Paused,
            "DELETED" => AdStatusEnum.Deleted,
            "ARCHIVED" => AdStatusEnum.Archived,
            _ => AdStatusEnum.Paused
        };

    private static DateTime ParseDateTimeOrDefault(string? value, DateTime defaultValue)
    {
        if (string.IsNullOrWhiteSpace(value)) return defaultValue;
        return DateTime.TryParse(value, out var result) ? result : defaultValue;
    }

    private static decimal ParseDecimalOrDefault(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        return decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result)
            ? result / 100m
            : 0;
    }
}
