using LegacyLens.Models;

namespace LegacyLens.Services;

public class ConsoleIndexReporter
{
	public void Print(CodebaseIndex codebaseIndex)
	{
		foreach (FileIndexEntry entry in codebaseIndex.Files)
		{
            if (entry.IsReadSuccessfully)
            {
                Console.WriteLine($"{entry.RelativePath} | {entry.Extension} | {entry.LineCount} lines | {entry.FunctionLikeCount} code items | {entry.SizeBytes} bytes");
            }
            else
            {
                Console.WriteLine($"{entry.RelativePath} | READ ERROR | {entry.ErrorMessage}");
            }
        }

		Console.WriteLine();
		Console.WriteLine("Summary:");
		Console.WriteLine($"Files: {codebaseIndex.Summary.FileCount}");
		Console.WriteLine($"Lines: {codebaseIndex.Summary.TotalLineCount}");
		Console.WriteLine($"Non-empty lines: {codebaseIndex.Summary.TotalNonEmptyLineCount}");
		Console.WriteLine($"Code items: {codebaseIndex.Summary.TotalFunctionLikeCount}");
		Console.WriteLine($"Size: {codebaseIndex.Summary.TotalSizeBytes} bytes");
        Console.WriteLine($"Read errors: {codebaseIndex.Summary.ReadErrorCount}");
    }
}