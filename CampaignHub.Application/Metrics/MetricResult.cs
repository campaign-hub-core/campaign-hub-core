namespace CampaignHub.Application.Metrics;

public record MetricResult(
    string Id,
    string CampaignId,
    DateTime ReferencePeriod,
    decimal Expenses,
    int Leads,
    string? Sales,
    string? Revenue,
    DateTime CreatedAt
);
