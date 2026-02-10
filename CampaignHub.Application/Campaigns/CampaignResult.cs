using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Application.Campaigns;

public record CampaignResult(
    string Id,
    string AdAccountId,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    CampaignStatusEnum CampaignStatus,
    DateTime CreatedAt
);
