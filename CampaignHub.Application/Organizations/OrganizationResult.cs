namespace CampaignHub.Application.Organizations;

public record OrganizationResult(
    string Id,
    string Name,
    bool Active,
    DateTime CreatedAt
);
