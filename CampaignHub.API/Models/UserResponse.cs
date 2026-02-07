namespace CampaignHub.API.Models;

public record UserResponse(string Id, string Name, string Email, bool Active, DateTime CreatedAt);
