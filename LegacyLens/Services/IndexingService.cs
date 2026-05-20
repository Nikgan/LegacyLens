using LegacyLens.Models;
using LegacyLens.Options;

namespace LegacyLens.Services;

public class IndexingService
{
    private readonly ScannerOptionsFactory _scannerOptionsFactory;
    private readonly CodebaseIndexBuilder _codebaseIndexBuilder;

    public IndexingService(
        ScannerOptionsFactory scannerOptionsFactory,
        CodebaseIndexBuilder codebaseIndexBuilder
    )
    {
        _scannerOptionsFactory = scannerOptionsFactory;
        _codebaseIndexBuilder = codebaseIndexBuilder;
    }

    public CodebaseIndex Build(string rootPath, SearchOption searchOption)
    {
        ScannerOptions scannerOptions = _scannerOptionsFactory.Create(searchOption);

        CodebaseIndex codebaseIndex = _codebaseIndexBuilder.Build(
            rootPath,
            scannerOptions
        );

        return codebaseIndex;
    }
}