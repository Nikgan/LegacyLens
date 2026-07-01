using LegacyLens.Api.Models;

namespace LegacyLens.Api.Middleware;

public class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ApiExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
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