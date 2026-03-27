namespace Workhours.Application.Exceptions;

public abstract class ApiException(string errorCode, string message, int statusCode) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;

    public int StatusCode { get; } = statusCode;
}