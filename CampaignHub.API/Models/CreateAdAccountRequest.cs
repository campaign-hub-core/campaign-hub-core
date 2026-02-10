using System.ComponentModel.DataAnnotations;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.API.Models;

public record CreateAdAccountRequest
{
    [Required(ErrorMessage = "O ID do cliente é obrigatório.")]
    [StringLength(100)]
    public required string CustomerId { get; init; }

    [Required(ErrorMessage = "O orçamento mensal é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O orçamento mensal deve ser maior que zero.")]
    public required decimal MonthlyBudget { get; init; }

    [Required(ErrorMessage = "O objetivo é obrigatório.")]
    [StringLength(500, MinimumLength = 1)]
    public required string Goal { get; init; }

    [Required(ErrorMessage = "A plataforma de anúncios é obrigatória.")]
    public required AdPlatformEnum AdPlatform { get; init; }
}
