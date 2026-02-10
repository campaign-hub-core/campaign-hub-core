using CampaignHub.Domain.Entities;

namespace CampaignHub.Infra.Repositories;

public interface IAdAccountRepository
{
    Task<AdAccount?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AdAccount>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
    Task<AdAccount> AddAsync(AdAccount adAccount, CancellationToken cancellationToken = default);
    void Remove(AdAccount adAccount);
}
