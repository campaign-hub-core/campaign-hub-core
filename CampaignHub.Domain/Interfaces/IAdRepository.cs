using CampaignHub.Domain.Entities;

namespace CampaignHub.Domain.Interfaces;

public interface IAdRepository
{
    Task<Ad?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
    Task<Ad> AddAsync(Ad ad, CancellationToken cancellationToken = default);
    void Update(Ad ad);
}
