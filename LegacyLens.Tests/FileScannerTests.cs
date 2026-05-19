using LegacyLens.Models;
using LegacyLens.Options;
using LegacyLens.Services;

namespace LegacyLens.Tests;

[TestClass]
public class FileScannerTests
{
	[TestMethod]
	public void Scan_ShouldFindAllowedFiles()
	{
		string rootPath = CreateTempDirectory();

		try
		{
			string sourceFilePath = Path.Combine(rootPath, "Program.cs");
			string textFilePath = Path.Combine(rootPath, "notes.txt");
			string ignoredFilePath = Path.Combine(rootPath, "image.png");

			File.WriteAllText(sourceFilePath, """
                public class Program
                {
                    public static void Main()
                    {
                    }
                }
                """);

			File.WriteAllText(textFilePath, "hello");
			File.WriteAllText(ignoredFilePath, "fake image");

			ScannerOptions options = new ScannerOptions()
			{
				AllowedPatterns = ["*.cs", "*.txt"],
				ExcludedDirectoryNames = ["bin", "obj"],
				SearchOption = SearchOption.AllDirectories
			};

			FileScanner scanner = new FileScanner();

			List<FileIndexEntry> entries = scanner.Scan(rootPath, options);

			Assert.HasCount(2, entries);

			bool hasProgramCs = entries.Any(entry => entry.RelativePath == "Program.cs");
			bool hasNotesTxt = entries.Any(entry => entry.RelativePath == "notes.txt");
			bool hasImagePng = entries.Any(entry => entry.RelativePath == "image.png");

			Assert.IsTrue(hasProgramCs);
			Assert.IsTrue(hasNotesTxt);
			Assert.IsFalse(hasImagePng);
		}
		finally
		{
			Directory.Delete(rootPath, true);
		}
	}

	[TestMethod]
	public void Scan_ShouldSkipExcludedDirectories()
	{
		string rootPath = CreateTempDirectory();

		try
		{
			string normalFilePath = Path.Combine(rootPath, "Main.cs");

			string objDirectoryPath = Path.Combine(rootPath, "obj");
			string binDirectoryPath = Path.Combine(rootPath, "bin");

			Directory.CreateDirectory(objDirectoryPath);
			Directory.CreateDirectory(binDirectoryPath);

			string objFilePath = Path.Combine(objDirectoryPath, "Generated.cs");
			string binFilePath = Path.Combine(binDirectoryPath, "Compiled.cs");

			File.WriteAllText(normalFilePath, "public class MainClass { }");
			File.WriteAllText(objFilePath, "public class GeneratedClass { }");
			File.WriteAllText(binFilePath, "public class CompiledClass { }");

			ScannerOptions options = new ScannerOptions()
			{
				AllowedPatterns = ["*.cs"],
				ExcludedDirectoryNames = ["bin", "obj"],
				SearchOption = SearchOption.AllDirectories
			};

			FileScanner scanner = new FileScanner();

			List<FileIndexEntry> entries = scanner.Scan(rootPath, options);

			Assert.HasCount(1, entries);
			Assert.AreEqual("Main.cs", entries[0].RelativePath);
		}
		finally
		{
			Directory.Delete(rootPath, true);
		}
	}

	[TestMethod]
	public void Scan_ShouldCalculateLineCountsAndCodeItems()
	{
		string rootPath = CreateTempDirectory();

		try
		{
			string sourceFilePath = Path.Combine(rootPath, "Program.cs");

			string fileContent = string.Join(Environment.NewLine, new[]
			{
			"public class Program",
			"{",
			"",
			"    public static void Main()",
			"    {",
			"    }",
			"}"
		});

			File.WriteAllText(sourceFilePath, fileContent);

			ScannerOptions options = new ScannerOptions()
			{
				AllowedPatterns = ["*.cs"],
				ExcludedDirectoryNames = ["bin", "obj"],
				SearchOption = SearchOption.AllDirectories
			};

			FileScanner scanner = new FileScanner();

			List<FileIndexEntry> entries = scanner.Scan(rootPath, options);

			Assert.HasCount(1, entries);

			FileIndexEntry entry = entries[0];

			Assert.AreEqual("Program.cs", entry.RelativePath);
			Assert.AreEqual(".cs", entry.Extension);
			Assert.AreEqual(7, entry.LineCount);
			Assert.AreEqual(6, entry.NonEmptyLineCount);
			Assert.AreEqual(1, entry.CodeItemCount);
			Assert.IsTrue(entry.IsReadSuccessfully);
			Assert.IsNull(entry.ErrorMessage);
		}
		finally
		{
			Directory.Delete(rootPath, true);
		}
	}

	private static string CreateTempDirectory()
	{
		string tempDirectoryPath = Path.Combine(
			Path.GetTempPath(),
			"LegacyLensTests",
			Guid.NewGuid().ToString()
		);

		Directory.CreateDirectory(tempDirectoryPath);

		return tempDirectoryPath;
	}
}