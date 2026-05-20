using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace LegacyLens.Api.Endpoints;

public static class SystemEndpoints
{
    public static void MapSystemEndpoints(this WebApplication app)
    {
        app.MapGet("/health", () =>
        {
            return Results.Ok(new
            {
                status = "ok",
                service = "LegacyLens.Api",
                timestamp = DateTime.UtcNow
            });
        })
        .WithName("Health")
        .WithTags("System")
        .Produces(StatusCodes.Status200OK);
    }
}