using System.ComponentModel.DataAnnotations;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.API.Models;

public record CreateCustomerRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, MinimumLength = 1)]
    public required string Name { get; init; }

    [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
    [StringLength(100)]
    public required string UserId { get; init; }

    [StringLength(1000)]
    public string? Observation { get; init; }

    [Required(ErrorMessage = "O tipo de cliente é obrigatório.")]
    public required CustomerTypeEnum CustomerType { get; init; }
}
