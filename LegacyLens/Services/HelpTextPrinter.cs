namespace LegacyLens.Services;

public class HelpTextPrinter
{
    public void Print()
    {
        Console.WriteLine("LegacyLens - codebase indexer");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  LegacyLens <rootPath> [--top|--recursive] [--output <filePath>]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  rootPath       Path to the directory that should be indexed.");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --top          Scan only the top directory.");
        Console.WriteLine("  --recursive    Scan the directory and all subdirectories. Default mode.");
        Console.WriteLine("  --output       Path to output JSON file. Must end with .json.");
        Console.WriteLine("  --help, -h     Show help.");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine(@"  LegacyLens ""C:\Project""");
        Console.WriteLine(@"  LegacyLens ""C:\Project"" --top");
        Console.WriteLine(@"  LegacyLens ""C:\Project"" --output ""C:\Temp\project-index.json""");
    }
}