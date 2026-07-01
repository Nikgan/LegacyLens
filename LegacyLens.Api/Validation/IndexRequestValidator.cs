using LegacyLens.Api.Models;

namespace LegacyLens.Api.Validation;

public class IndexRequestValidator
{
    public IResult? Validate(IndexRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RootPath))
        {
            return Results.BadRequest(new ApiError()
            {
                Error = "RootPath is required."
            });
        }

        if (!Directory.Exists(request.RootPath))
        {
            return Results.NotFound(new ApiError()
            {
                Error = $"Directory not found: {request.RootPath}"
            });
        }

        return null;
    }
}