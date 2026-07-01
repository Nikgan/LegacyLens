using LegacyLens.Api.Models;
using LegacyLens.Api.Validation;
using LegacyLens.Models;
using LegacyLens.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace LegacyLens.Api.Endpoints;

public static class IndexingEndpoints
{
    public static void MapIndexingEndpoints(this WebApplication app)
    {
        app.MapPost("/index", BuildIndex)
            .WithName("BuildIndex")
            .WithTags("Indexing")
            .Accepts<IndexRequest>("application/json")
            .Produces<CodebaseIndex>(StatusCodes.Status200OK)
            .Produces<ApiError>(StatusCodes.Status400BadRequest)
            .Produces<ApiError>(StatusCodes.Status404NotFound);

        app.MapPost("/index/summary", BuildIndexSummary)
            .WithName("BuildIndexSummary")
            .WithTags("Indexing")
            .Accepts<IndexRequest>("application/json")
            .Produces<IndexSummary>(StatusCodes.Status200OK)
            .Produces<ApiError>(StatusCodes.Status400BadRequest)
            .Produces<ApiError>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> BuildIndex(
        IndexRequest request,
        IndexingService indexingService,
        IndexRequestValidator validator,
        CancellationToken cancellationToken
    )
    {
        IResult? validationError = validator.Validate(request);

        if (validationError is not null)
        {
            return validationError;
        }

        SearchOption searchOption = GetSearchOption(request);

        CodebaseIndex codebaseIndex = await indexingService.BuildAsync(
            request.RootPath,
            searchOption,
            cancellationToken
        );

        return Results.Ok(codebaseIndex);
    }

    private static async Task<IResult> BuildIndexSummary(
        IndexRequest request,
        IndexingService indexingService,
        IndexRequestValidator validator,
        CancellationToken cancellationToken
    )
    {
        IResult? validationError = validator.Validate(request);

        if (validationError is not null)
        {
            return validationError;
        }

        SearchOption searchOption = GetSearchOption(request);

        CodebaseIndex codebaseIndex = await indexingService.BuildAsync(
            request.RootPath,
            searchOption,
            cancellationToken
        );

        return Results.Ok(codebaseIndex.Summary);
    }

    private static SearchOption GetSearchOption(IndexRequest request)
    {
        return request.Recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;
    }
}