using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Workhours.Services;

namespace Workhours.Infrastructure.Auth;

public sealed class DistributedCacheTokenBlacklistService(IDistributedCache cache) : ITokenBlacklistService
{
    public async Task RevokeTokenAsync(string token, DateTimeOffset expiresAtUtc, CancellationToken cancellationToken)
    {
        var ttl = expiresAtUtc - DateTimeOffset.UtcNow;
        if (ttl <= TimeSpan.Zero)
        {
            return;
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };

        await cache.SetStringAsync(BuildCacheKey(token), "revoked", options, cancellationToken);
    }

    public async Task<bool> IsTokenRevokedAsync(string token, CancellationToken cancellationToken)
    {
        var revokedValue = await cache.GetStringAsync(BuildCacheKey(token), cancellationToken);
        return revokedValue is not null;
    }

    private static string BuildCacheKey(string token)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return $"token-blacklist:{Convert.ToHexString(hashBytes)}";
    }
}