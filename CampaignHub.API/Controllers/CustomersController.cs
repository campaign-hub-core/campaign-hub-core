using CampaignHub.API.Models;
using CampaignHub.Application.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var input = new CreateCustomerInput(request.Name, request.UserId, request.Observation, request.CustomerType);
            var result = await _customerService.CreateAsync(input, cancellationToken);

            return Created($"/api/customers/{result.Id}", result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("usuário"))
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Busca um cliente pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await _customerService.GetByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Busca clientes pelo nome (busca parcial, case-insensitive).
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CustomerResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByName([FromQuery] string name, CancellationToken cancellationToken)
    {
        var result = await _customerService.GetByNameAsync(name, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Atualiza nome e observação de um cliente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomerResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var input = new UpdateCustomerInput(request.Name, request.Observation);
        var result = await _customerService.UpdateAsync(id, input, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Realiza o delete lógico de um cliente (status → Inactive).
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await _customerService.DeleteAsync(id, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Ativa um cliente (status → Active).
    /// </summary>
    [HttpPost("{id}/enable")]
    [ProducesResponseType(typeof(CustomerResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Enable(string id, CancellationToken cancellationToken)
    {
        var result = await _customerService.EnableAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Suspende um cliente (status → Suspended).
    /// </summary>
    [HttpPost("{id}/pause")]
    [ProducesResponseType(typeof(CustomerResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pause(string id, CancellationToken cancellationToken)
    {
        var result = await _customerService.PauseAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Desativa um cliente (status → Inactive).
    /// </summary>
    [HttpPost("{id}/disable")]
    [ProducesResponseType(typeof(CustomerResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Disable(string id, CancellationToken cancellationToken)
    {
        var result = await _customerService.DisableAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
