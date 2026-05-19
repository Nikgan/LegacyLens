namespace LegacyLens.Options;

public record CommandLineOptions
{
    public required string RootPath { get; init; }
    public required SearchOption SearchOption { get; init; }
    public required string OutputPath { get; init; }
    public required bool ShowHelp { get; init; }
}