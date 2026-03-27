namespace Workhours.Application.Exceptions;

public sealed class NotFoundException(string message) : ApiException("not_found", message, StatusCodes.Status404NotFound);