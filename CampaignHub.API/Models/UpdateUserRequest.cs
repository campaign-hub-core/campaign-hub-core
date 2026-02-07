using System.ComponentModel.DataAnnotations;

namespace CampaignHub.API.Models;

public record UpdateUserRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, MinimumLength = 1)]
    public required string Name { get; init; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(200)]
    public required string Email { get; init; }
}
