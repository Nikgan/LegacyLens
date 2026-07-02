using LegacyLens.Models;

namespace LegacyLens.Infrastructure.Storage.Entities;

public class IndexedCodeItemEntity
{
    public long Id { get; set; }

    public long IndexedFileId { get; set; }

    public IndexedFileEntity IndexedFile { get; set; } = null!;

    public CodeItemKind Kind { get; set; }

    public string Name { get; set; } = "";

    public int LineNumber { get; set; }

    public int EndLineNumber { get; set; }

    public string Signature { get; set; } = "";

    public List<IndexedCodeChunkEntity> CodeChunks { get; set; } = [];
}