namespace LegacyLens.Options;

public record ScannerOptions
{
    public required string[] AllowedPatterns { get; init; }
    public required string[] ExcludedDirectoryNames { get; init; }
    public SearchOption SearchOption { get; init; } = SearchOption.AllDirectories;
}