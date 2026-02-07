namespace CampaignHub.Application.Users;

public record UserResult(
    string Id,
    string Name,
    string Email,
    bool Active,
    DateTime CreatedAt
);
