using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Interfaces;

public interface IAdAccountRepository
{
    Task<AdAccount?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<AdAccount?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AdAccount>> GetByPlatformAsync(AdPlatformEnum platform, CancellationToken cancellationToken = default);
    Task<AdAccount> AddAsync(AdAccount adAccount, CancellationToken cancellationToken = default);
    void Update(AdAccount adAccount);
}
