namespace CampaignHub.Application.AdAccounts;

public interface IAdAccountService
{
    Task<AdAccountResult> CreateAsync(CreateAdAccountInput input, CancellationToken cancellationToken = default);
    Task<AdAccountResult?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AdAccountResult>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
    Task<AdAccountResult?> UpdateAsync(string id, UpdateAdAccountInput input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
