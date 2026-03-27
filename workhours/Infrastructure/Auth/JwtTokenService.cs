using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Workhours.Domain;
using Workhours.Services;

namespace Workhours.Infrastructure.Auth;

public sealed class JwtTokenService(IConfiguration configuration) : ITokenService
{
    private readonly JwtSecurityTokenHandler tokenHandler = new();

    public string CreateAccessToken(ApplicationUser user)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer must be configured.");
        var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Jwt:Audience must be configured.");
        var signingKey = jwtSection["SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey must be configured.");
        var lifetimeMinutes = jwtSection.GetValue<int?>("AccessTokenLifetimeMinutes") ?? 15;
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(lifetimeMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? user.Email ?? user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAtUtc,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public DateTimeOffset GetExpirationUtc(string token)
    {
        var jwt = tokenHandler.ReadJwtToken(token);
        return new DateTimeOffset(jwt.ValidTo, TimeSpan.Zero);
    }
}