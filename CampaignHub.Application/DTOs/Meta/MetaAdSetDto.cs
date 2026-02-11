namespace CampaignHub.Application.DTOs.Meta;

public record MetaAdSetDto(
    string Id,
    string CampaignId,
    string Name,
    string Status,
    string? DailyBudget
);
