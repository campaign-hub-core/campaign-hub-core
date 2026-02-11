namespace CampaignHub.Application.DTOs.Meta;

public record MetaInsightDto(
    string CampaignId,
    string DateStart,
    string DateStop,
    string? Spend,
    int Leads,
    int Purchases,
    string? PurchaseValue
);
