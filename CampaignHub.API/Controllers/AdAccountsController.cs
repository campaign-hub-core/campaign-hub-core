using CampaignHub.API.Models;
using CampaignHub.Application.AdAccounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignHub.API.Controllers;

[ApiController]
[Route("api/ad-accounts")]
[Authorize]
public class AdAccountsController : ControllerBase
{
    private readonly IAdAccountService _adAccountService;

    public AdAccountsController(IAdAccountService adAccountService)
    {
        _adAccountService = adAccountService;
    }

    /// <summary>
    /// Cria uma nova conta de anúncios.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AdAccountResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAdAccountRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var input = new CreateAdAccountInput(request.CustomerId, request.MonthlyBudget, request.Goal, request.AdPlatform);
            var result = await _adAccountService.CreateAsync(input, cancellationToken);

            return Created($"/api/ad-accounts/{result.Id}", result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Busca uma conta de anúncios pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AdAccountResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await _adAccountService.GetByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Lista contas de anúncios de um cliente.
    /// </summary>
    [HttpGet("by-customer/{customerId}")]
    [ProducesResponseType(typeof(IEnumerable<AdAccountResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomerId(string customerId, CancellationToken cancellationToken)
    {
        var result = await _adAccountService.GetByCustomerIdAsync(customerId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Atualiza orçamento e objetivo de uma conta de anúncios.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AdAccountResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateAdAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var input = new UpdateAdAccountInput(request.MonthlyBudget, request.Goal);
            var result = await _adAccountService.UpdateAsync(id, input, cancellationToken);

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
    /// Remove uma conta de anúncios (cascade deleta campanhas e métricas).
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await _adAccountService.DeleteAsync(id, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
