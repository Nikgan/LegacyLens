using LegacyLens.Api.Validation;
using LegacyLens.Options;
using LegacyLens.Services;

namespace LegacyLens.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLegacyLensIndexing(
        this IServiceCollection services
    )
    {
        services.AddSingleton<ScannerOptionsFactory>();
        services.AddSingleton<CodeItemExtractor>();
        services.AddSingleton<CodeChunkBuilder>();
        services.AddSingleton<FileScanner>();
        services.AddSingleton<IndexSummaryBuilder>();
        services.AddSingleton<CodebaseIndexBuilder>();
        services.AddSingleton<IndexingService>();
        services.AddSingleton<IndexRequestValidator>();

        return services;
    }
}