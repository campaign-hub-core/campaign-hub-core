using CampaignHub.Domain.Entities;

namespace CampaignHub.Domain.Interfaces;

public interface IMetricCampaignRepository
{
    Task<MetricCampaign?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MetricCampaign>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default);
    Task<MetricCampaign?> GetByCampaignAndPeriodAsync(string campaignId, DateTime referencePeriod, CancellationToken cancellationToken = default);
    Task<MetricCampaign> AddAsync(MetricCampaign metric, CancellationToken cancellationToken = default);
    void Update(MetricCampaign metric);
    void Remove(MetricCampaign metric);
}
