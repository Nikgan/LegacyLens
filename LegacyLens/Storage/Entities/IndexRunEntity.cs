namespace LegacyLens.Storage.Entities;

public class IndexRunEntity
{
    public long Id { get; set; }

    public string RootPath { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; }

    public int FileCount { get; set; }

    public int ReadErrorCount { get; set; }

    public int TotalLineCount { get; set; }

    public int TotalNonEmptyLineCount { get; set; }

    public int TotalCodeItemCount { get; set; }

    public int TotalCodeChunkCount { get; set; }

    public long TotalSizeBytes { get; set; }

    public List<IndexedFileEntity> Files { get; set; } = [];
}