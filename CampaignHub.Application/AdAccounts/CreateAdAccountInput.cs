using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Application.AdAccounts;

public record CreateAdAccountInput(string CustomerId, decimal MonthlyBudget, string Goal, AdPlatformEnum AdPlatform);
