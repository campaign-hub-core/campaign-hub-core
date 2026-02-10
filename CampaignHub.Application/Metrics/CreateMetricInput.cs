namespace CampaignHub.Application.Metrics;

public record CreateMetricInput(string CampaignId, DateTime ReferencePeriod, decimal Expenses, int Leads, string? Sales, string? Revenue);
