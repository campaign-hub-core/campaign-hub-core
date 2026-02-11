using CampaignHub.Domain.Entities;

namespace CampaignHub.Domain.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Organization>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default);
}
