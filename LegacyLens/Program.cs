using LegacyLens.Models;
using LegacyLens.Services;

string rootPath = args.Length > 0
    ? args[0]
    : Directory.GetCurrentDirectory();

string[] allowedPatterns = ["*.prg", "*.ch", "*.cs", "*.log", "*.txt", "*.md"];
string[] excludedDirectoryNames = ["bin", "obj", ".git", ".vs", ".vscode", "node_modules", "packages"];

Console.WriteLine($"Indexing folder: {rootPath}");

if (!Directory.Exists(rootPath))
{
    Console.WriteLine($"Folder not found: {rootPath}");
    return;
}

FileScanner fileScanner = new();
IndexSummaryBuilder summaryBuilder = new();
ConsoleIndexReporter reporter = new();
IndexJsonWriter jsonWriter = new();

List<FileIndexEntry> entries = fileScanner.Scan(rootPath, allowedPatterns, excludedDirectoryNames);
IndexSummary summary = summaryBuilder.Build(entries);

CodebaseIndex codebaseIndex = new(
    rootPath,
    DateTime.UtcNow,
    summary,
    entries
);

reporter.Print(codebaseIndex);
jsonWriter.Save(codebaseIndex, rootPath);