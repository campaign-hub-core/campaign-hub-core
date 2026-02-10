using System.ComponentModel.DataAnnotations;

namespace CampaignHub.API.Models;

public record LoginRequest
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public required string Password { get; init; }
}
