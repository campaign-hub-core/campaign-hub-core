using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampaignHub.Infra.Repositories;

public class AdRepository : IAdRepository
{
    private readonly AppDbContext _context;

    public AdRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Ad?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default) =>
        await _context.Ads
            .FirstOrDefaultAsync(a => a.ExternalId == externalId, cancellationToken);

    public async Task<Ad> AddAsync(Ad ad, CancellationToken cancellationToken = default)
    {
        await _context.Ads.AddAsync(ad, cancellationToken);
        return ad;
    }

    public void Update(Ad ad)
    {
        _context.Ads.Update(ad);
    }
}
