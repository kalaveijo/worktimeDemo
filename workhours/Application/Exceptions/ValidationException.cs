namespace Workhours.Application.Exceptions;

public sealed class ValidationException(string message) : ApiException("validation_error", message, StatusCodes.Status400BadRequest);