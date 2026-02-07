using CampaignHub.Domain.Entities;
using CampaignHub.Infra.Repositories;

namespace CampaignHub.Application.Organizations;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrganizationService(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork)
    {
        _organizationRepository = organizationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrganizationResult> CreateAsync(CreateOrganizationInput input, CancellationToken cancellationToken = default)
    {
        var organization = new Organization(input.Name.Trim());

        await _organizationRepository.AddAsync(organization, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(organization);
    }

    public async Task<OrganizationResult?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(id, cancellationToken);

        if (organization is null || !organization.Active)
            return null;

        return ToResult(organization);
    }

    public async Task<IEnumerable<OrganizationResult>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var organizations = await _organizationRepository.GetByNameAsync(name, cancellationToken);
        return organizations.Select(ToResult);
    }

    public async Task<OrganizationResult?> UpdateAsync(string id, UpdateOrganizationInput input, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(id, cancellationToken);

        if (organization is null || !organization.Active)
            return null;

        organization.Update(input.Name.Trim());
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(organization);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(id, cancellationToken);

        if (organization is null || !organization.Active)
            return false;

        organization.Disable();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static OrganizationResult ToResult(Organization organization)
    {
        return new OrganizationResult(
            organization.Id,
            organization.Name,
            organization.Active,
            organization.CreatedAt
        );
    }
}
