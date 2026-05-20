using System.Text.Json;
using System.Text.Json.Serialization;
using LegacyLens.Api.Models;
using LegacyLens.Models;
using LegacyLens.Options;
using LegacyLens.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
    );
});

builder.Services.AddSingleton<ScannerOptionsFactory>();
builder.Services.AddSingleton<CodeItemExtractor>();
builder.Services.AddSingleton<CodeChunkBuilder>();
builder.Services.AddSingleton<FileScanner>();
builder.Services.AddSingleton<IndexSummaryBuilder>();
builder.Services.AddSingleton<CodebaseIndexBuilder>();
builder.Services.AddSingleton<IndexingService>();

WebApplication app = builder.Build();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        service = "LegacyLens.Api",
        timestamp = DateTime.UtcNow
    });
});

app.MapPost(
    "/index",
    (IndexRequest request, IndexingService indexingService) =>
    {
        if (string.IsNullOrWhiteSpace(request.RootPath))
        {
            return Results.BadRequest(new
            {
                error = "RootPath is required."
            });
        }

        if (!Directory.Exists(request.RootPath))
        {
            return Results.NotFound(new
            {
                error = $"Directory not found: {request.RootPath}"
            });
        }

        SearchOption searchOption = request.Recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        CodebaseIndex codebaseIndex = indexingService.Build(
            request.RootPath,
            searchOption
        );

        return Results.Ok(codebaseIndex);
    }
);

app.Run();