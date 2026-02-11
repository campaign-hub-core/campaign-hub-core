using CampaignHub.Domain.Entities;

namespace CampaignHub.Domain.Interfaces;

public interface IMetricCampaignRepository
{
    Task<MetricCampaign?> GetByCampaignAndPeriodAsync(string campaignId, DateTime referencePeriod, CancellationToken cancellationToken = default);
    Task<MetricCampaign> AddAsync(MetricCampaign metric, CancellationToken cancellationToken = default);
    void Update(MetricCampaign metric);
}
