namespace CampaignHub.Application.Customers;

public interface ICustomerService
{
    Task<CustomerResult> CreateAsync(CreateCustomerInput input, CancellationToken cancellationToken = default);
    Task<CustomerResult?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomerResult>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<CustomerResult?> UpdateAsync(string id, UpdateCustomerInput input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<CustomerResult?> EnableAsync(string id, CancellationToken cancellationToken = default);
    Task<CustomerResult?> PauseAsync(string id, CancellationToken cancellationToken = default);
    Task<CustomerResult?> DisableAsync(string id, CancellationToken cancellationToken = default);
}
