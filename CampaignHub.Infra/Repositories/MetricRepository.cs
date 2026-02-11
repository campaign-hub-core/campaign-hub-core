using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampaignHub.Infra.Repositories;

public class MetricRepository : IMetricCampaignRepository
{
    private readonly AppDbContext _context;

    public MetricRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MetricCampaign?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await _context.MetricCampaigns.FindAsync([id], cancellationToken);

    public async Task<IEnumerable<MetricCampaign>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default) =>
        await _context.MetricCampaigns
            .AsNoTracking()
            .Where(m => m.CampaignId == campaignId)
            .OrderBy(m => m.ReferencePeriod)
            .ToListAsync(cancellationToken);

    public async Task<MetricCampaign?> GetByCampaignAndPeriodAsync(string campaignId, DateTime referencePeriod, CancellationToken cancellationToken = default)
    {
        var normalizedPeriod = new DateTime(referencePeriod.Year, referencePeriod.Month, 1);
        return await _context.MetricCampaigns
            .FirstOrDefaultAsync(m => m.CampaignId == campaignId && m.ReferencePeriod == normalizedPeriod, cancellationToken);
    }

    public async Task<MetricCampaign> AddAsync(MetricCampaign metric, CancellationToken cancellationToken = default)
    {
        await _context.MetricCampaigns.AddAsync(metric, cancellationToken);
        return metric;
    }

    public void Update(MetricCampaign metric)
    {
        _context.MetricCampaigns.Update(metric);
    }

    public void Remove(MetricCampaign metric)
    {
        _context.MetricCampaigns.Remove(metric);
    }
}
