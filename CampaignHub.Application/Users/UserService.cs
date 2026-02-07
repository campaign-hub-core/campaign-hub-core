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
}
