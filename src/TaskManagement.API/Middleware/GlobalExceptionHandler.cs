using Microsoft.AspNetCore.Diagnostics;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Models;

namespace TaskManagement.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                "One or more validation failures have occurred.",
                validationEx.Errors.SelectMany(kvp => kvp.Value.Select(v => $"{kvp.Key}: {v}"))),

            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                notFoundEx.Message,
                Enumerable.Empty<string>()),

            ForbiddenAccessException forbiddenEx => (
                StatusCodes.Status403Forbidden,
                forbiddenEx.Message,
                Enumerable.Empty<string>()),

            ConflictException conflictEx => (
                StatusCodes.Status409Conflict,
                conflictEx.Message,
                Enumerable.Empty<string>()),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized.",
                Enumerable.Empty<string>()),

            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred. Please try again later.",
                Enumerable.Empty<string>())
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);
        }

        var response = ApiResponse.Fail(message, statusCode, errors);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
