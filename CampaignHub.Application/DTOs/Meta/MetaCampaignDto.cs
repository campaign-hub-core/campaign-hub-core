namespace CampaignHub.Application.DTOs.Meta;

public record MetaCampaignDto(
    string Id,
    string Name,
    string Status,
    string? StartTime,
    string? StopTime
);
