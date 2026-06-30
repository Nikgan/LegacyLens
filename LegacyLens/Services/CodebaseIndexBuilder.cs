using LegacyLens.Models;
using LegacyLens.Options;

namespace LegacyLens.Services;

public class CodebaseIndexBuilder
{
    private readonly FileScanner _fileScanner;
    private readonly IndexSummaryBuilder _summaryBuilder;

    public CodebaseIndexBuilder(
        FileScanner fileScanner,
        IndexSummaryBuilder summaryBuilder)
    {
        _fileScanner = fileScanner;
        _summaryBuilder = summaryBuilder;
    }

    public CodebaseIndex Build(string rootPath, ScannerOptions scannerOptions)
    {
        List<FileIndexEntry> entries = _fileScanner.Scan(rootPath, scannerOptions);
        IndexSummary summary = _summaryBuilder.Build(entries);

        CodebaseIndex codebaseIndex = new CodebaseIndex(
            rootPath,
            DateTime.UtcNow,
            summary,
            entries
        );

        return codebaseIndex;
    }

    public async Task<CodebaseIndex> BuildAsync(string rootPath, ScannerOptions scannerOptions, CancellationToken cancellationToken)
    {
        List<FileIndexEntry> entries = await _fileScanner.ScanAsync(rootPath, scannerOptions, cancellationToken);
        IndexSummary summary = _summaryBuilder.Build(entries);

        CodebaseIndex codebaseIndex = new CodebaseIndex(
            rootPath,
            DateTime.UtcNow,
            summary,
            entries
        );

        return codebaseIndex;
    }
}