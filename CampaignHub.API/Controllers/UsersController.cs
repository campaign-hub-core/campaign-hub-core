using CampaignHub.API.Models;
using CampaignHub.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace CampaignHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var input = new CreateUserInput(request.Name, request.Email, request.Password);
            var user = await _userService.CreateAsync(input, cancellationToken);

            var response = new UserResponse(
                user.Id,
                user.Name,
                user.Email,
                user.Active,
                user.CreatedAt);

            return Created($"/api/users/{user.Id}", response);
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
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);

        if (user is null)
            return NotFound();

        var response = new UserResponse(user.Id, user.Name, user.Email, user.Active, user.CreatedAt);
        return Ok(response);
    }

    /// <summary>
    /// Busca usuários pelo nome (busca parcial, case-insensitive).
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByName([FromQuery] string name, CancellationToken cancellationToken)
    {
        var users = await _userService.GetByNameAsync(name, cancellationToken);
        var response = users.Select(u => new UserResponse(u.Id, u.Name, u.Email, u.Active, u.CreatedAt));
        return Ok(response);
    }

    /// <summary>
    /// Atualiza nome e e-mail de um usuário (sem alterar senha).
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var input = new UpdateUserInput(request.Name, request.Email);
        var user = await _userService.UpdateAsync(id, input, cancellationToken);

        if (user is null)
            return NotFound();

        var response = new UserResponse(user.Id, user.Name, user.Email, user.Active, user.CreatedAt);
        return Ok(response);
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
