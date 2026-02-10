using CampaignHub.Domain.Entities;

namespace CampaignHub.Infra.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default);
}
