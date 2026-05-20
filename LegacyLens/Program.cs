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


SearchOption searchOption = commandLineOptions.SearchOption;

Console.WriteLine($"Indexing folder: {rootPath}");
Console.WriteLine($"Search mode: {searchOption}");
Console.WriteLine($"Output path: {commandLineOptions.OutputPath}");

if (!Directory.Exists(rootPath))
{
    Console.WriteLine($"Folder not found: {rootPath}");
    return;
}

ScannerOptionsFactory scannerOptionsFactory = new ScannerOptionsFactory();
CodeItemExtractor codeItemExtractor = new();
CodeChunkBuilder codeChunkBuilder = new();
FileScanner fileScanner = new(codeItemExtractor, codeChunkBuilder);
IndexSummaryBuilder summaryBuilder = new();
CodebaseIndexBuilder indexBuilder = new(fileScanner, summaryBuilder);
IndexingService indexingService = new(scannerOptionsFactory, indexBuilder);

ConsoleIndexReporter reporter = new();
IndexJsonWriter jsonWriter = new();

CodebaseIndex codebaseIndex = indexingService.Build(rootPath, searchOption);

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