namespace LegacyLens.Storage.Entities;

public class IndexedFileEntity
{
    public long Id { get; set; }

    public long IndexRunId { get; set; }

    public IndexRunEntity IndexRun { get; set; } = null!;

    public string FullPath { get; set; } = "";

    public string RelativePath { get; set; } = "";

    public string Extension { get; set; } = "";

    public long SizeBytes { get; set; }

    public int LineCount { get; set; }

    public int NonEmptyLineCount { get; set; }

    public string? ErrorMessage { get; set; }

    public List<IndexedCodeItemEntity> CodeItems { get; set; } = [];

    public List<IndexedCodeChunkEntity> CodeChunks { get; set; } = [];
}