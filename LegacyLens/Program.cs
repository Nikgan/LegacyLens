using LegacyLens.Models;
using LegacyLens.Options;
using LegacyLens.Services;

CommandLineOptionsParser commandLineOptionsParser = new CommandLineOptionsParser();
HelpTextPrinter helpTextPrinter = new HelpTextPrinter();

CommandLineOptions commandLineOptions;

try
{
    commandLineOptions = commandLineOptionsParser.Parse(args);
}
catch (ArgumentException exception)
{
    Console.WriteLine($"Argument error: {exception.Message}");
    Console.WriteLine("Use --help to see available options.");
    return;
}

if (commandLineOptions.ShowHelp)
{
    helpTextPrinter.Print();
    return;
}


string rootPath = commandLineOptions.RootPath;


ScannerOptions scannerOptions = new()
{
    AllowedPatterns = ["*.prg", "*.ch", "*.cs", "*.log", "*.txt", "*.md"],
    ExcludedDirectoryNames = ["bin", "obj", ".git", ".vs", ".vscode", "node_modules", "packages"],
    SearchOption = commandLineOptions.SearchOption
};

Console.WriteLine($"Indexing folder: {rootPath}");
Console.WriteLine($"Search mode: {scannerOptions.SearchOption}");
Console.WriteLine($"Output path: {commandLineOptions.OutputPath}");

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
try
{
    jsonWriter.Save(codebaseIndex, commandLineOptions.OutputPath);
}
catch (Exception exception) when (
    exception is IOException ||
    exception is UnauthorizedAccessException ||
    exception is DirectoryNotFoundException ||
    exception is PathTooLongException ||
    exception is NotSupportedException ||
    exception is ArgumentException)
{
    Console.WriteLine($"Output error: {exception.Message}");
    return;
}