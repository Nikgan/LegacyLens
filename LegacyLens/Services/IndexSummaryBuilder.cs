using LegacyLens.Models;

namespace LegacyLens.Services;

public class IndexSummaryBuilder
{
    public IndexSummary Build(List<FileIndexEntry> entries)
    {
        int fileCount = entries.Count;
        int readErrorCount = 0;
        int totalLineCount = 0;
        int totalNonEmptyLineCount = 0;
        int totalCodeItemCount = 0;
        int totalCodeChunkCount = 0;
        long totalSizeBytes = 0;

        foreach (FileIndexEntry entry in entries)
        {
            if (!entry.IsReadSuccessfully)
            {
                readErrorCount++;
            }

            totalLineCount += entry.LineCount;
            totalNonEmptyLineCount += entry.NonEmptyLineCount;
            totalCodeItemCount += entry.CodeItemCount;
            totalCodeChunkCount += entry.CodeChunkCount;
            totalSizeBytes += entry.SizeBytes;
        }

        IndexSummary summary = new IndexSummary(
            fileCount,
            readErrorCount,
            totalLineCount,
            totalNonEmptyLineCount,
            totalCodeItemCount,
            totalCodeChunkCount,
            totalSizeBytes
        );

        return summary;
    }
}