namespace LegacyLens.Infrastructure.Storage.Entities;

public class IndexedCodeChunkEntity
{
    public long Id { get; set; }

    public long IndexedFileId { get; set; }

    public IndexedFileEntity IndexedFile { get; set; } = null!;

    public long? IndexedCodeItemId { get; set; }

    public IndexedCodeItemEntity? IndexedCodeItem { get; set; }

    public int ChunkIndex { get; set; }

    public string Name { get; set; } = "";

    public int StartLineNumber { get; set; }

    public int EndLineNumber { get; set; }

    public string Text { get; set; } = "";
}