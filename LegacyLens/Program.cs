using LegacyLens.Models;
using LegacyLens.Services;
using LegacyLens.Options;

string rootPath = args.Length > 0
    ? args[0]
    : Directory.GetCurrentDirectory();

ScannerOptions scannerOptions = new ScannerOptions()
{
    AllowedPatterns = ["*.prg", "*.ch", "*.cs", "*.log", "*.txt", "*.md"],
    ExcludedDirectoryNames = ["bin", "obj", ".git", ".vs", ".vscode", "node_modules", "packages"],
    SearchOption = SearchOption.AllDirectories
};

Console.WriteLine($"Indexing folder: {rootPath}");

if (!Directory.Exists(rootPath))
{
    Console.WriteLine($"Folder not found: {rootPath}");
    return;
}

FileScanner fileScanner = new FileScanner();
IndexSummaryBuilder summaryBuilder = new IndexSummaryBuilder();
CodebaseIndexBuilder indexBuilder = new CodebaseIndexBuilder(fileScanner, summaryBuilder);

ConsoleIndexReporter reporter = new ConsoleIndexReporter();
IndexJsonWriter jsonWriter = new IndexJsonWriter();

CodebaseIndex codebaseIndex = indexBuilder.Build(rootPath, scannerOptions);

reporter.Print(codebaseIndex);
jsonWriter.Save(codebaseIndex, rootPath);
;