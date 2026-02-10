using CampaignHub.API.Models;
using CampaignHub.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Realiza o login e retorna um token JWT.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var input = new LoginInput(request.Email, request.Password);
        var result = await _authService.LoginAsync(input, cancellationToken);

        if (result is null)
            return Unauthorized(new { message = "E-mail ou senha inválidos." });

        return Ok(result);
    }
}
