using System.ComponentModel.DataAnnotations;

namespace CampaignHub.API.Models;

public record UpdateCustomerRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, MinimumLength = 1)]
    public required string Name { get; init; }

    [StringLength(1000)]
    public string? Observation { get; init; }
}
