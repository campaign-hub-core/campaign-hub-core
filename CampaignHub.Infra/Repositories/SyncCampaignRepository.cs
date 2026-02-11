using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampaignHub.Infra.Repositories;

public class SyncCampaignRepository : Domain.Interfaces.ICampaignRepository
{
    private readonly AppDbContext _context;

    public SyncCampaignRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Campaign?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default) =>
        await _context.Campaigns
            .FirstOrDefaultAsync(c => c.ExternalId == externalId, cancellationToken);

    public async Task<IEnumerable<Campaign>> GetByAdAccountIdAsync(string adAccountId, CancellationToken cancellationToken = default) =>
        await _context.Campaigns
            .Where(c => c.AdAccountId == adAccountId)
            .ToListAsync(cancellationToken);

    public async Task<Campaign> AddAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        await _context.Campaigns.AddAsync(campaign, cancellationToken);
        return campaign;
    }

    public void Update(Campaign campaign)
    {
        _context.Campaigns.Update(campaign);
    }
}
