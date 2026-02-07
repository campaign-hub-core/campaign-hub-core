using System.ComponentModel.DataAnnotations;

namespace CampaignHub.API.Models;

public record UpdateOrganizationRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, MinimumLength = 1)]
    public required string Name { get; init; }
}
