using CampaignHub.Domain.Entities;
using CampaignHub.Infra.Repositories;

namespace CampaignHub.Application.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> CreateAsync(CreateUserInput input, CancellationToken cancellationToken = default)
    {
        var emailNormalized = input.Email.Trim().ToLowerInvariant();

        var existing = await _userRepository.GetByEmailAsync(emailNormalized, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("Já existe um usuário cadastrado com este e-mail.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(input.Password);
        var user = new User(input.Name.Trim(), emailNormalized, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null || !user.Active)
            return null;

        return user;
    }

    public async Task<IEnumerable<User>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetByNameAsync(name, cancellationToken);
    }

    public async Task<User?> UpdateAsync(string id, UpdateUserInput input, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null || !user.Active)
            return null;

        user.Update(input.Name.Trim(), input.Email.Trim().ToLowerInvariant());
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null || !user.Active)
            return false;

        user.Disable();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
