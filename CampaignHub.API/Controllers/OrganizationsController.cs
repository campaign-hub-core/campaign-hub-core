using CampaignHub.API.Models;
using CampaignHub.Application.Organizations;
using Microsoft.AspNetCore.Mvc;

namespace CampaignHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    /// <summary>
    /// Cria uma nova organização.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrganizationResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var input = new CreateOrganizationInput(request.Name);
        var result = await _organizationService.CreateAsync(input, cancellationToken);

        return Created($"/api/organizations/{result.Id}", result);
    }

    /// <summary>
    /// Busca uma organização pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrganizationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await _organizationService.GetByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Busca organizações pelo nome (busca parcial, case-insensitive).
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<OrganizationResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByName([FromQuery] string name, CancellationToken cancellationToken)
    {
        var result = await _organizationService.GetByNameAsync(name, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Atualiza o nome de uma organização.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OrganizationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateOrganizationRequest request, CancellationToken cancellationToken)
    {
        var input = new UpdateOrganizationInput(request.Name);
        var result = await _organizationService.UpdateAsync(id, input, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Realiza o delete lógico de uma organização (desativa).
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await _organizationService.DeleteAsync(id, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
