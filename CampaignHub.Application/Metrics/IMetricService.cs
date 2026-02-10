namespace CampaignHub.Application.Metrics;

public interface IMetricService
{
    Task<MetricResult> CreateAsync(CreateMetricInput input, CancellationToken cancellationToken = default);
    Task<MetricResult?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MetricResult>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default);
    Task<MetricResult?> UpdateAsync(string id, UpdateMetricInput input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
