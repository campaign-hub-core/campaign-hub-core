namespace CampaignHub.Application.DTOs.User;

public record UserResponseDto(
    string Id,
    string Name,
    string Email,
    bool Active,
    DateTime CreatedAt
);
