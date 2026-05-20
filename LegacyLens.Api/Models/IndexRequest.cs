namespace LegacyLens.Api.Models;

public record IndexRequest
{
    public required string RootPath { get; init; }
    public bool Recursive { get; init; } = true;
}