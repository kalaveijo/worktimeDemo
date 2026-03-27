using System.Text.Json;
using Workhours.Api.Models;
using Workhours.Application.Exceptions;

namespace Workhours.Api.Middleware;

public sealed class ApiExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiException exception)
        {
            await WriteErrorAsync(context, exception.StatusCode, exception.ErrorCode, exception.Message);
        }
        catch (Exception)
        {
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "server_error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string errorCode, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new ErrorResponse(errorCode, message));
        await context.Response.WriteAsync(payload);
    }
}