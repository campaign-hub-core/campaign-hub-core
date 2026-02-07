using CampaignHub.Domain.Entities;

namespace CampaignHub.Application.Users;

public interface IUserService
{
    Task<User> CreateAsync(CreateUserInput input, CancellationToken cancellationToken = default);
}
