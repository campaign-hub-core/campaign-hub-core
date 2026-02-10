using System.ComponentModel.DataAnnotations;

namespace CampaignHub.API.Models;

public record UpdateAdAccountRequest
{
    [Required(ErrorMessage = "O orçamento mensal é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O orçamento mensal deve ser maior que zero.")]
    public required decimal MonthlyBudget { get; init; }

    [Required(ErrorMessage = "O objetivo é obrigatório.")]
    [StringLength(500, MinimumLength = 1)]
    public required string Goal { get; init; }
}
