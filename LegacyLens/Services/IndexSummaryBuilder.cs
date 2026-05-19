using LegacyLens.Models;

namespace LegacyLens.Services;

public class IndexSummaryBuilder
{
    public IndexSummary Build(List<FileIndexEntry> entries)
    {
        int readErrorCount = 0;
        int totalLineCount = 0;
        int totalNonEmptyLineCount = 0;
        int totalFunctionLikeCount = 0;
        long totalSizeBytes = 0;

        foreach (FileIndexEntry entry in entries)
        {
            if (!entry.IsReadSuccessfully)
            {
                readErrorCount++;
            }

            totalLineCount += entry.LineCount;
            totalNonEmptyLineCount += entry.NonEmptyLineCount;
            totalFunctionLikeCount += entry.FunctionLikeCount;
            totalSizeBytes += entry.SizeBytes;
        }

        IndexSummary summary = new IndexSummary(
            entries.Count,
            readErrorCount,
            totalLineCount,
            totalNonEmptyLineCount,
            totalFunctionLikeCount,
            totalSizeBytes
        );

        return summary;
    }
}