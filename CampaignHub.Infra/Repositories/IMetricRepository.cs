using CampaignHub.Domain.Entities;

namespace CampaignHub.Infra.Repositories;

public interface IMetricRepository
{
    Task<MetricCampaign?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MetricCampaign>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default);
    Task<MetricCampaign?> GetByCampaignAndPeriodAsync(string campaignId, DateTime referencePeriod, CancellationToken cancellationToken = default);
    Task<MetricCampaign> AddAsync(MetricCampaign metric, CancellationToken cancellationToken = default);
    void Remove(MetricCampaign metric);
}
