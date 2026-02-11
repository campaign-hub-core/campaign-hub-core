using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampaignHub.Infra.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly AppDbContext _context;

    public OrganizationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await _context.Organizations.FindAsync([id], cancellationToken);

    public async Task<IEnumerable<Organization>> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await _context.Organizations
            .AsNoTracking()
            .Where(o => o.Name.ToLower().Contains(name.ToLower()) && o.Active)
            .ToListAsync(cancellationToken);

    public async Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        await _context.Organizations.AddAsync(organization, cancellationToken);
        return organization;
    }
}
