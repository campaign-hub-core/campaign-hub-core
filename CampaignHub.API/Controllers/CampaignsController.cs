using CampaignHub.API.Models;
using CampaignHub.Application.Campaigns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public CampaignsController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    /// <summary>
    /// Cria uma nova campanha.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CampaignResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCampaignRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var input = new CreateCampaignInput(request.AdAccountId, request.Name, request.StartDate, request.EndDate);
            var result = await _campaignService.CreateAsync(input, cancellationToken);

            return Created($"/api/campaigns/{result.Id}", result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Busca uma campanha pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CampaignResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await _campaignService.GetByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Busca campanhas pelo nome (busca parcial, case-insensitive).
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CampaignResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByName([FromQuery] string name, CancellationToken cancellationToken)
    {
        var result = await _campaignService.GetByNameAsync(name, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Lista campanhas de uma conta de anúncios.
    /// </summary>
    [HttpGet("by-ad-account/{adAccountId}")]
    [ProducesResponseType(typeof(IEnumerable<CampaignResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAdAccountId(string adAccountId, CancellationToken cancellationToken)
    {
        var result = await _campaignService.GetByAdAccountIdAsync(adAccountId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Atualiza nome e datas de uma campanha.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CampaignResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateCampaignRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var input = new UpdateCampaignInput(request.Name, request.StartDate, request.EndDate);
            var result = await _campaignService.UpdateAsync(id, input, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove uma campanha.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await _campaignService.DeleteAsync(id, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Pausa uma campanha (status → Paused).
    /// </summary>
    [HttpPost("{id}/pause")]
    [ProducesResponseType(typeof(CampaignResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Pause(string id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _campaignService.PauseAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Ativa uma campanha (status → Active).
    /// </summary>
    [HttpPost("{id}/activate")]
    [ProducesResponseType(typeof(CampaignResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Activate(string id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _campaignService.ActivateAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Conclui uma campanha (status → Completed).
    /// </summary>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(typeof(CampaignResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(string id, CancellationToken cancellationToken)
    {
        var result = await _campaignService.CompleteAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
