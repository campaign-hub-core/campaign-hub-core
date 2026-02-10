using CampaignHub.API.Models;
using CampaignHub.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Cria um novo usuário.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var input = new CreateUserInput(request.Name, request.Email, request.Password);
            var result = await _userService.CreateAsync(input, cancellationToken);

            return Created($"/api/users/{result.Id}", result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("e-mail"))
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Busca um usuário pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Busca usuários pelo nome (busca parcial, case-insensitive).
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<UserResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByName([FromQuery] string name, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByNameAsync(name, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Atualiza nome e e-mail de um usuário (sem alterar senha).
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var input = new UpdateUserInput(request.Name, request.Email);
        var result = await _userService.UpdateAsync(id, input, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Realiza o delete lógico de um usuário (desativa).
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteAsync(id, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
