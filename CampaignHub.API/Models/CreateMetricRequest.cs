using System.ComponentModel.DataAnnotations;

namespace CampaignHub.API.Models;

public record CreateMetricRequest
{
    [Required(ErrorMessage = "O ID da campanha é obrigatório.")]
    [StringLength(100)]
    public required string CampaignId { get; init; }

    [Required(ErrorMessage = "O período de referência é obrigatório.")]
    public required DateTime ReferencePeriod { get; init; }

    [Required(ErrorMessage = "As despesas são obrigatórias.")]
    [Range(0, double.MaxValue, ErrorMessage = "As despesas não podem ser negativas.")]
    public required decimal Expenses { get; init; }

    [Required(ErrorMessage = "O número de leads é obrigatório.")]
    [Range(0, int.MaxValue, ErrorMessage = "O número de leads não pode ser negativo.")]
    public required int Leads { get; init; }

    [StringLength(100)]
    public string? Sales { get; init; }

    [StringLength(100)]
    public string? Revenue { get; init; }
}
