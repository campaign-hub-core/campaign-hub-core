using CampaignHub.Domain.Entities;

namespace CampaignHub.Domain.Interfaces;

public interface IAdSetRepository
{
    Task<AdSet?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AdSet>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default);
    Task<AdSet> AddAsync(AdSet adSet, CancellationToken cancellationToken = default);
    void Update(AdSet adSet);
}
