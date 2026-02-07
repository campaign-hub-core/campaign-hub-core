namespace CampaignHub.Application.Users;

public interface IUserService
{
    Task<UserResult> CreateAsync(CreateUserInput input, CancellationToken cancellationToken = default);
    Task<UserResult?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserResult>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<UserResult?> UpdateAsync(string id, UpdateUserInput input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
