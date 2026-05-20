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

            CodeItemExtractor codeItemExtractor = new CodeItemExtractor();
            FileScanner scanner = new FileScanner(codeItemExtractor);

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

            CodeItemExtractor codeItemExtractor = new CodeItemExtractor();
            FileScanner scanner = new FileScanner(codeItemExtractor);

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

            CodeItemExtractor codeItemExtractor = new CodeItemExtractor();
            FileScanner scanner = new FileScanner(codeItemExtractor);

            List<FileIndexEntry> entries = scanner.Scan(rootPath, options);

			Assert.HasCount(1, entries);

			FileIndexEntry entry = entries[0];

			Assert.AreEqual("Program.cs", entry.RelativePath);
			Assert.AreEqual(".cs", entry.Extension);
			Assert.AreEqual(7, entry.LineCount);
			Assert.AreEqual(6, entry.NonEmptyLineCount);
			Assert.AreEqual(1, entry.CodeItemCount);
            Assert.HasCount(1, entry.CodeItems);
            Assert.AreEqual(CodeItemKind.Class, entry.CodeItems[0].Kind);
            Assert.AreEqual("Program", entry.CodeItems[0].Name);
            Assert.AreEqual(1, entry.CodeItems[0].LineNumber);
            Assert.AreEqual("public class Program", entry.CodeItems[0].Signature);
            Assert.IsTrue(entry.IsReadSuccessfully);
			Assert.IsNull(entry.ErrorMessage);
		}
		finally
		{
			Directory.Delete(rootPath, true);
		}
	}

	[TestMethod]
	public void Scan_ShouldFindFilesInSubdirectories_WhenSearchOptionIsAllDirectories()
	{
		string rootPath = CreateTempDirectory();

		try
		{
			string servicesDirectoryPath = Path.Combine(rootPath, "src", "Services");
			Directory.CreateDirectory(servicesDirectoryPath);

			string sourceFilePath = Path.Combine(servicesDirectoryPath, "FileScanner.cs");

			File.WriteAllText(sourceFilePath, "public class FileScanner { }");

			ScannerOptions options = new()
			{
				AllowedPatterns = ["*.cs"],
				ExcludedDirectoryNames = ["bin", "obj"],
				SearchOption = SearchOption.AllDirectories
			};

            CodeItemExtractor codeItemExtractor = new CodeItemExtractor();
            FileScanner scanner = new FileScanner(codeItemExtractor);

            List<FileIndexEntry> entries = scanner.Scan(rootPath, options);

			Assert.HasCount(1, entries);

			string expectedRelativePath = Path.Combine("src", "Services", "FileScanner.cs");

			Assert.AreEqual(expectedRelativePath, entries[0].RelativePath);
			Assert.AreEqual(1, entries[0].CodeItemCount);
		}
		finally
		{
			Directory.Delete(rootPath, true);
		}
	}

	[TestMethod]
	public void Scan_ShouldNotFindFilesInSubdirectories_WhenSearchOptionIsTopDirectoryOnly()
	{
		string rootPath = CreateTempDirectory();

		try
		{
			string rootFilePath = Path.Combine(rootPath, "Program.cs");

			string servicesDirectoryPath = Path.Combine(rootPath, "src", "Services");
			Directory.CreateDirectory(servicesDirectoryPath);

			string nestedFilePath = Path.Combine(servicesDirectoryPath, "FileScanner.cs");

			File.WriteAllText(rootFilePath, "public class Program { }");
			File.WriteAllText(nestedFilePath, "public class FileScanner { }");

			ScannerOptions options = new ScannerOptions()
			{
				AllowedPatterns = ["*.cs"],
				ExcludedDirectoryNames = ["bin", "obj"],
				SearchOption = SearchOption.TopDirectoryOnly
			};

            CodeItemExtractor codeItemExtractor = new CodeItemExtractor();
            FileScanner scanner = new FileScanner(codeItemExtractor);

            List<FileIndexEntry> entries = scanner.Scan(rootPath, options);

			Assert.HasCount(1, entries);
			Assert.AreEqual("Program.cs", entries[0].RelativePath);
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