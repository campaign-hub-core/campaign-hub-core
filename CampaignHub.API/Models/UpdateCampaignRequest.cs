using System.ComponentModel.DataAnnotations;

namespace CampaignHub.API.Models;

public record UpdateCampaignRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, MinimumLength = 1)]
    public required string Name { get; init; }

    [Required(ErrorMessage = "A data de início é obrigatória.")]
    public required DateTime StartDate { get; init; }

    [Required(ErrorMessage = "A data de término é obrigatória.")]
    public required DateTime EndDate { get; init; }
}
