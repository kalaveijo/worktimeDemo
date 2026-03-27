using Workhours.Domain;

namespace Workhours.Services;

public interface ITokenService
{
    string CreateAccessToken(ApplicationUser user);

    DateTimeOffset GetExpirationUtc(string token);
}