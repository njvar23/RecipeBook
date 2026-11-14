using Microsoft.AspNetCore.Mvc;
using RecipeBook.Api.DTOs.Auth;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Creates a new account with the "User" role.</summary>
    [HttpPost("register")]
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Register(RegisterRequest request, CancellationToken ct)
    {
        var user = await authService.RegisterAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, user);
    }

    /// <summary>Returns a JWT access token and its expiry time.</summary>
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        return Ok(await authService.LoginAsync(request, ct));
    }
}
