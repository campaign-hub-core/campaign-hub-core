using CampaignHub.Domain.Entities;

namespace CampaignHub.Application.Users;

public interface IUserService
{
    Task<User> CreateAsync(CreateUserInput input, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<User?> UpdateAsync(string id, UpdateUserInput input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
