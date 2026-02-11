using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampaignHub.Infra.Repositories;

public class CampaignRepository : ICampaignRepository
{
    private readonly AppDbContext _context;

    public CampaignRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Campaign?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await _context.Campaigns.FindAsync([id], cancellationToken);

    public async Task<Campaign?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default) =>
        await _context.Campaigns
            .FirstOrDefaultAsync(c => c.ExternalId == externalId, cancellationToken);

    public async Task<IEnumerable<Campaign>> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await _context.Campaigns
            .AsNoTracking()
            .Where(c => c.Name.ToLower().Contains(name.ToLower()) && c.CampaignStatus != CampaignStatusEnum.Cancelled)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Campaign>> GetByAdAccountIdAsync(string adAccountId, CancellationToken cancellationToken = default) =>
        await _context.Campaigns
            .AsNoTracking()
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

    public void Remove(Campaign campaign)
    {
        _context.Campaigns.Remove(campaign);
    }
}
