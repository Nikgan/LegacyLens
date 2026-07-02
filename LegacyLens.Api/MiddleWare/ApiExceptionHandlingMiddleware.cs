using LegacyLens.Api.Models;

namespace LegacyLens.Api.Middleware;

public class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

    public ApiExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionHandlingMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogDebug(
                "Request was cancelled by the client. Method: {Method}, Path: {Path}",
                context.Request.Method,
                context.Request.Path
            );

            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing request. Method: {Method}, Path: {Path}",
                context.Request.Method,
                context.Request.Path
            );

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            ApiError error = new ApiError()
            {
                Error = "Unexpected server error."
            };

            await context.Response.WriteAsJsonAsync(error);
        }
    }
}