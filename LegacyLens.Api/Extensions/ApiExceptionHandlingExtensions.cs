using LegacyLens.Api.Middleware;

namespace LegacyLens.Api.Extensions;

public static class ApiExceptionHandlingExtensions
{
    public static WebApplication UseApiExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<ApiExceptionHandlingMiddleware>();

        return app;
    }
}