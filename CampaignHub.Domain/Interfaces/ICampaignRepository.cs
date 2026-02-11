using CampaignHub.Domain.Entities;

namespace CampaignHub.Domain.Interfaces;

public interface ICampaignRepository
{
    Task<Campaign?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Campaign?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetByAdAccountIdAsync(string adAccountId, CancellationToken cancellationToken = default);
    Task<Campaign> AddAsync(Campaign campaign, CancellationToken cancellationToken = default);
    void Update(Campaign campaign);
    void Remove(Campaign campaign);
}
