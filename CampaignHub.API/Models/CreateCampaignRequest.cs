using System.ComponentModel.DataAnnotations;

namespace CampaignHub.API.Models;

public record CreateCampaignRequest
{
    [Required(ErrorMessage = "O ID da conta de anúncios é obrigatório.")]
    [StringLength(100)]
    public required string AdAccountId { get; init; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, MinimumLength = 1)]
    public required string Name { get; init; }

    [Required(ErrorMessage = "A data de início é obrigatória.")]
    public required DateTime StartDate { get; init; }

    [Required(ErrorMessage = "A data de término é obrigatória.")]
    public required DateTime EndDate { get; init; }
}
