namespace LegacyLens.Models;

public record FileIndexEntry
{
    public required string FullPath { get; init; }
    public required string RelativePath { get; init; }
    public required string Extension { get; init; }
    public required long SizeBytes { get; init; }
    public required int LineCount { get; init; }
    public required int NonEmptyLineCount { get; init; }
    public required int CodeItemCount { get; init; }
    public required string? ErrorMessage { get; init; }

    public bool IsReadSuccessfully => ErrorMessage is null;
}