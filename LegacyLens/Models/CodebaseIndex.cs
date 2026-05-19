namespace LegacyLens.Models;

public record CodebaseIndex(
    string RootPath,
    DateTime CreatedAt,
    IndexSummary Summary,
    List<FileIndexEntry> Files
);