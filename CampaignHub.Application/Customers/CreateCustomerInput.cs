using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Application.Customers;

public record CreateCustomerInput(string Name, string UserId, string? Observation, CustomerTypeEnum CustomerType);
