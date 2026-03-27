using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workhours.Api.Extensions;
using Workhours.Api.Models.Auth;
using Workhours.Application.Exceptions;
using Workhours.Domain;
using Workhours.Services;

namespace Workhours.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    ITokenBlacklistService tokenBlacklistService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            throw new ConflictException("An account with this email already exists.");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString("N"),
            UserName = request.Email,
            Email = request.Email,
            NormalizedUserName = request.Email.ToUpperInvariant(),
            NormalizedEmail = request.Email.ToUpperInvariant(),
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new ValidationException(string.Join(" ", result.Errors.Select(x => x.Description)));
        }

        return CreatedAtAction(nameof(RegisterAsync), new { id = user.Id }, null);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var validPassword = await userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var accessToken = tokenService.CreateAccessToken(user);
        var expiresAtUtc = tokenService.GetExpirationUtc(accessToken);

        return Ok(new LoginResponse(accessToken, expiresAtUtc));
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedException("Bearer token is required.");
        }

        var token = authHeader["Bearer ".Length..].Trim();
        var expiresAtUtc = tokenService.GetExpirationUtc(token);
        await tokenBlacklistService.RevokeTokenAsync(token, expiresAtUtc, cancellationToken);

        return NoContent();
    }
}