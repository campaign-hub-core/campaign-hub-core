namespace CampaignHub.Application.Metrics;

public record UpdateMetricInput(decimal Expenses, int Leads, string? Sales, string? Revenue);
