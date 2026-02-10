namespace CampaignHub.Application.Auth;

public record LoginResult(string Token, DateTime ExpiresAt, string UserId, string UserName);
