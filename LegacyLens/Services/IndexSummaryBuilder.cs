using LegacyLens.Models;

namespace LegacyLens.Services;

public class IndexSummaryBuilder
{
    public IndexSummary Build(List<FileIndexEntry> entries)
    {
        int readErrorCount = 0;
        int totalLineCount = 0;
        int totalNonEmptyLineCount = 0;
        int TotalCodeItemCount = 0;
        long totalSizeBytes = 0;

        foreach (FileIndexEntry entry in entries)
        {
            if (!entry.IsReadSuccessfully)
            {
                readErrorCount++;
            }

            totalLineCount += entry.LineCount;
            totalNonEmptyLineCount += entry.NonEmptyLineCount;
            TotalCodeItemCount += entry.CodeItemCount;
            totalSizeBytes += entry.SizeBytes;
        }

        IndexSummary summary = new IndexSummary(
            entries.Count,
            readErrorCount,
            totalLineCount,
            totalNonEmptyLineCount,
            TotalCodeItemCount,
            totalSizeBytes
        );

        return summary;
    }
}