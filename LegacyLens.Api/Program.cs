using LegacyLens.Api.Endpoints;
using LegacyLens.Api.Extensions;
using LegacyLens.Options;
using LegacyLens.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
    );
});

builder.Services.AddLegacyLensIndexing();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseApiExceptionHandling();

app.MapSystemEndpoints();
app.MapIndexingEndpoints();

app.Run();