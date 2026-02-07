using CampaignHub.Application.DTOs.User;
using CampaignHub.Infra;
using Microsoft.EntityFrameworkCore;

namespace CampaignHub.Application.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponseDto?> GetByIdAsync(string id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && u.Active);

        if (user is null)
            return null;

        return ToDto(user);
    }

    public async Task<IEnumerable<UserResponseDto>> GetByNameAsync(string name)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => u.Name.ToLower().Contains(name.ToLower()) && u.Active)
            .ToListAsync();

        return users.Select(ToDto);
    }

    public async Task<UserResponseDto?> UpdateAsync(string id, UpdateUserDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Active);

        if (user is null)
            return null;

        user.Update(dto.Name, dto.Email);
        await _context.SaveChangesAsync();

        return ToDto(user);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Active);

        if (user is null)
            return false;

        user.Disable();
        await _context.SaveChangesAsync();

        return true;
    }

    private static UserResponseDto ToDto(Domain.Entities.User user)
    {
        return new UserResponseDto(
            user.Id,
            user.Name,
            user.Email,
            user.Active,
            user.CreatedAt
        );
    }
}
