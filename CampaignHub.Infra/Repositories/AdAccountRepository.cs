using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampaignHub.Infra.Repositories;

public class AdAccountRepository : IAdAccountRepository
{
    private readonly AppDbContext _context;

    public AdAccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdAccount?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await _context.AdAccounts.FindAsync([id], cancellationToken);

    public async Task<AdAccount?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default) =>
        await _context.AdAccounts
            .FirstOrDefaultAsync(a => a.ExternalId == externalId, cancellationToken);

    public async Task<IEnumerable<AdAccount>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default) =>
        await _context.AdAccounts
            .AsNoTracking()
            .Where(a => a.CustomerId == customerId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<AdAccount>> GetByPlatformAsync(AdPlatformEnum platform, CancellationToken cancellationToken = default) =>
        await _context.AdAccounts
            .Where(a => a.AdPlatform == platform)
            .ToListAsync(cancellationToken);

    public async Task<AdAccount> AddAsync(AdAccount adAccount, CancellationToken cancellationToken = default)
    {
        await _context.AdAccounts.AddAsync(adAccount, cancellationToken);
        return adAccount;
    }

    public void Update(AdAccount adAccount)
    {
        _context.AdAccounts.Update(adAccount);
    }

    public void Remove(AdAccount adAccount)
    {
        _context.AdAccounts.Remove(adAccount);
    }
}
