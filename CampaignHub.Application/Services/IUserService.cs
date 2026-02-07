using CampaignHub.Application.DTOs.User;

namespace CampaignHub.Application.Services;

public interface IUserService
{
    Task<UserResponseDto?> GetByIdAsync(string id);
    Task<IEnumerable<UserResponseDto>> GetByNameAsync(string name);
    Task<UserResponseDto?> UpdateAsync(string id, UpdateUserDto dto);
    Task<bool> DeleteAsync(string id);
}
