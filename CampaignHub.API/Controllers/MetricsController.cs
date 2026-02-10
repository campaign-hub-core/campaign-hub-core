using CampaignHub.API.Models;
using CampaignHub.Application.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MetricsController : ControllerBase
{
    private readonly IMetricService _metricService;

    public MetricsController(IMetricService metricService)
    {
        _metricService = metricService;
    }

    /// <summary>
    /// Cria uma nova métrica de campanha.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MetricResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateMetricRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var input = new CreateMetricInput(request.CampaignId, request.ReferencePeriod, request.Expenses, request.Leads, request.Sales, request.Revenue);
            var result = await _metricService.CreateAsync(input, cancellationToken);

            return Created($"/api/metrics/{result.Id}", result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Busca uma métrica pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MetricResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await _metricService.GetByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Lista métricas de uma campanha.
    /// </summary>
    [HttpGet("by-campaign/{campaignId}")]
    [ProducesResponseType(typeof(IEnumerable<MetricResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCampaignId(string campaignId, CancellationToken cancellationToken)
    {
        var result = await _metricService.GetByCampaignIdAsync(campaignId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Atualiza uma métrica de campanha.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MetricResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateMetricRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var input = new UpdateMetricInput(request.Expenses, request.Leads, request.Sales, request.Revenue);
            var result = await _metricService.UpdateAsync(id, input, cancellationToken);

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
    /// Remove uma métrica de campanha.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await _metricService.DeleteAsync(id, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
