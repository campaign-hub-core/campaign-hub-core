using CampaignHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampaignHub.Infra.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await _context.Users.FindAsync([id], cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<IEnumerable<User>> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await _context.Users
            .AsNoTracking()
            .Where(u => u.Name.ToLower().Contains(name.ToLower()) && u.Active)
            .ToListAsync(cancellationToken);

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        return user;
    }
}
