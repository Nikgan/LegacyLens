namespace LegacyLens.Models;

public record CodeItem
{
    public required CodeItemKind Kind { get; init; }
    public required string Name { get; init; }
    public required int LineNumber { get; init; }
    public required string Signature { get; init; }
}