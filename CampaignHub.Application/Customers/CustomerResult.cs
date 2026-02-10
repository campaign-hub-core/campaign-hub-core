using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Application.Customers;

public record CustomerResult(
    string Id,
    string Name,
    string UserId,
    string? Observation,
    CustomerTypeEnum CustomerType,
    CustomerStatusEnum CustomerStatus,
    DateTime CreatedAt
);
