namespace LegacyLens.Api.Models;

public record ApiError
{
    public required string Error { get; init; }
}