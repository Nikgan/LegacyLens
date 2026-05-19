namespace LegacyLens.Models;

public record IndexSummary(
    int FileCount,
    int ReadErrorCount,
    int TotalLineCount,
    int TotalNonEmptyLineCount,
    int totalCodeItemCount,
    long TotalSizeBytes
);