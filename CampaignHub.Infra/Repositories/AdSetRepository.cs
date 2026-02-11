using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampaignHub.Infra.Repositories;

public class AdSetRepository : IAdSetRepository
{
    private readonly AppDbContext _context;

    public AdSetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdSet?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default) =>
        await _context.AdSets
            .FirstOrDefaultAsync(a => a.ExternalId == externalId, cancellationToken);

    public async Task<IEnumerable<AdSet>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default) =>
        await _context.AdSets
            .Where(a => a.CampaignId == campaignId)
            .ToListAsync(cancellationToken);

    public async Task<AdSet> AddAsync(AdSet adSet, CancellationToken cancellationToken = default)
    {
        await _context.AdSets.AddAsync(adSet, cancellationToken);
        return adSet;
    }

    public void Update(AdSet adSet)
    {
        _context.AdSets.Update(adSet);
    }
}
