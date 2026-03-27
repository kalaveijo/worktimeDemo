namespace Workhours.Application.Exceptions;

public sealed class ConflictException(string message) : ApiException("conflict", message, StatusCodes.Status409Conflict);