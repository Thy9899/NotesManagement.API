using System.Net;
using System.Text.Json;

namespace NotesManagement.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // Processes the HTTP request and handles any unhandled exceptions.
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unhandled exception occurred."
            );

            await HandleExceptionAsync(context, ex);
        }
    }

    // Creates and returns a standardized JSON error response.
    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message = exception switch
            {
                ArgumentException =>
                    exception.Message,

                KeyNotFoundException =>
                    exception.Message,

                UnauthorizedAccessException =>
                    "You are not authorized to perform this action.",

                _ =>
                    "An unexpected error occurred."
            }
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response)
        );
    }
}