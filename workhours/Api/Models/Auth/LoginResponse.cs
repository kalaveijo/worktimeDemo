namespace Workhours.Api.Models.Auth;

public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAtUtc);