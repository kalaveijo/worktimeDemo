namespace Workhours.Application.Exceptions;

public sealed class UnauthorizedException(string message) : ApiException("unauthorized", message, StatusCodes.Status401Unauthorized);