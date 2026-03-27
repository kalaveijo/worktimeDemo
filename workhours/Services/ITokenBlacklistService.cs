namespace Workhours.Services;

public interface ITokenBlacklistService
{
    Task RevokeTokenAsync(string token, DateTimeOffset expiresAtUtc, CancellationToken cancellationToken);

    Task<bool> IsTokenRevokedAsync(string token, CancellationToken cancellationToken);
}