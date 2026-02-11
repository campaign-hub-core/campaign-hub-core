using CampaignHub.Application.DTOs.Meta;

namespace CampaignHub.Application.Interfaces;

public interface IMetaAdsApiClient
{
    Task<IReadOnlyList<MetaCampaignDto>> GetCampaignsAsync(string adAccountExternalId, CancellationToken ct = default);
    Task<IReadOnlyList<MetaAdSetDto>> GetAdSetsAsync(string campaignExternalId, CancellationToken ct = default);
    Task<IReadOnlyList<MetaAdDto>> GetAdsAsync(string adSetExternalId, CancellationToken ct = default);
    Task<IReadOnlyList<MetaInsightDto>> GetCampaignInsightsAsync(string adAccountExternalId, DateOnly since, DateOnly until, CancellationToken ct = default);
}
