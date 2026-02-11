using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Interfaces;

namespace CampaignHub.Application.Metrics;

public class MetricService : IMetricService
{
    private readonly IMetricCampaignRepository _metricRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MetricService(IMetricCampaignRepository metricRepository, ICampaignRepository campaignRepository, IUnitOfWork unitOfWork)
    {
        _metricRepository = metricRepository;
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MetricResult> CreateAsync(CreateMetricInput input, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetByIdAsync(input.CampaignId, cancellationToken);
        if (campaign is null)
            throw new InvalidOperationException("A campanha informada não existe.");

        if (input.Expenses < 0)
            throw new InvalidOperationException("As despesas não podem ser negativas.");

        if (input.Leads < 0)
            throw new InvalidOperationException("O número de leads não pode ser negativo.");

        var existing = await _metricRepository.GetByCampaignAndPeriodAsync(input.CampaignId, input.ReferencePeriod, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("Já existe uma métrica para esta campanha no período informado.");

        var metric = new MetricCampaign(input.CampaignId, input.ReferencePeriod, input.Expenses, input.Leads, input.Sales?.Trim(), input.Revenue?.Trim());

        await _metricRepository.AddAsync(metric, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(metric);
    }

    public async Task<MetricResult?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var metric = await _metricRepository.GetByIdAsync(id, cancellationToken);

        if (metric is null)
            return null;

        return ToResult(metric);
    }

    public async Task<IEnumerable<MetricResult>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default)
    {
        var metrics = await _metricRepository.GetByCampaignIdAsync(campaignId, cancellationToken);
        return metrics.Select(ToResult);
    }

    public async Task<MetricResult?> UpdateAsync(string id, UpdateMetricInput input, CancellationToken cancellationToken = default)
    {
        var metric = await _metricRepository.GetByIdAsync(id, cancellationToken);

        if (metric is null)
            return null;

        if (input.Expenses < 0)
            throw new InvalidOperationException("As despesas não podem ser negativas.");

        if (input.Leads < 0)
            throw new InvalidOperationException("O número de leads não pode ser negativo.");

        metric.Update(input.Expenses, input.Leads, input.Sales?.Trim(), input.Revenue?.Trim());
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResult(metric);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var metric = await _metricRepository.GetByIdAsync(id, cancellationToken);

        if (metric is null)
            return false;

        _metricRepository.Remove(metric);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static MetricResult ToResult(MetricCampaign metric)
    {
        return new MetricResult(
            metric.Id,
            metric.CampaignId,
            metric.ReferencePeriod,
            metric.Expenses,
            metric.Leads,
            metric.Sales,
            metric.Revenue,
            metric.CreatedAt
        );
    }
}
