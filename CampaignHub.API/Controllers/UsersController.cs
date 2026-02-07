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
}
