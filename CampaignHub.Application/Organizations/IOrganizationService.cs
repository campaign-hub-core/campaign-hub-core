namespace CampaignHub.Application.Organizations;

public interface IOrganizationService
{
    Task<OrganizationResult> CreateAsync(CreateOrganizationInput input, CancellationToken cancellationToken = default);
    Task<OrganizationResult?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrganizationResult>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<OrganizationResult?> UpdateAsync(string id, UpdateOrganizationInput input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
