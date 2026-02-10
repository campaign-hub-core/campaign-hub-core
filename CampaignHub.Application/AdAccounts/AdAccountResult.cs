using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Application.AdAccounts;

public record AdAccountResult(
    string Id,
    string CustomerId,
    decimal MonthlyBudget,
    string Goal,
    AdPlatformEnum AdPlatform,
    DateTime CreatedAt
);
