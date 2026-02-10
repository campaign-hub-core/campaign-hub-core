namespace CampaignHub.Application.Auth;

public interface IAuthService
{
    Task<LoginResult?> LoginAsync(LoginInput input, CancellationToken cancellationToken = default);
}
