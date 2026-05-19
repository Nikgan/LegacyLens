namespace LegacyLens.Models;

public record IndexSummary(
    int FileCount,
    int ReadErrorCount,
    int TotalLineCount,
    int TotalNonEmptyLineCount,
    int TotalFunctionLikeCount,
    long TotalSizeBytes
);