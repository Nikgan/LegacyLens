using LegacyLens.Models;
using LegacyLens.Options;
using LegacyLens.Services;

namespace LegacyLens.Tests;

[TestClass]
public class FileScannerIntegrationTests
{
    [TestMethod]
    public void Scan_ShouldIndexRealisticHarbourFileWithCodeItemsAndChunks()
    {
        string rootPath = Path.Combine(
            Path.GetTempPath(),
            $"LegacyLensTests_{Guid.NewGuid()}"
        );

        try
        {
            Directory.CreateDirectory(rootPath);

            string filePath = Path.Combine(rootPath, "InputField.prg");

            File.WriteAllLines(
                filePath,
                [
                    "#include \"hbclass.ch\"",
                    "",
                    "class InputField",
                    "    data xValue",
                    "    method New()",
                    "endclass",
                    "",
                    "method New(xValue) class InputField",
                    "    ::xValue := xValue",
                    "return Self",
                    "",
                    "function CreateInputField()",
                    "    return InputField():New('')"
                ]
            );

            ScannerOptions options = new ScannerOptions()
            {
                AllowedPatterns = ["*.prg"],
                ExcludedDirectoryNames = ["bin", "obj", ".git", ".vs"],
                SearchOption = SearchOption.AllDirectories
            };

            CodeItemExtractor codeItemExtractor = new CodeItemExtractor();
            CodeChunkBuilder codeChunkBuilder = new CodeChunkBuilder();
            FileScanner scanner = new FileScanner(codeItemExtractor, codeChunkBuilder);

            List<FileIndexEntry> entries = scanner.Scan(rootPath, options);

            Assert.HasCount(1, entries);

            FileIndexEntry entry = entries[0];

            Assert.AreEqual("InputField.prg", entry.RelativePath);
            Assert.AreEqual(".prg", entry.Extension);
            Assert.AreEqual(13, entry.LineCount);
            Assert.AreEqual(10, entry.NonEmptyLineCount);

            Assert.AreEqual("InputField.prg#class:InputField:3-7", entry.CodeChunks[0].Id);
            Assert.AreEqual("InputField.prg#method:New:8-11", entry.CodeChunks[1].Id);
            Assert.AreEqual("InputField.prg#function:CreateInputField:12-13", entry.CodeChunks[2].Id);

            Assert.HasCount(3, entry.CodeItems);

            Assert.AreEqual(CodeItemKind.Class, entry.CodeItems[0].Kind);
            Assert.AreEqual("InputField", entry.CodeItems[0].Name);
            Assert.AreEqual(3, entry.CodeItems[0].LineNumber);

            Assert.AreEqual(CodeItemKind.Method, entry.CodeItems[1].Kind);
            Assert.AreEqual("New", entry.CodeItems[1].Name);
            Assert.AreEqual(8, entry.CodeItems[1].LineNumber);

            Assert.AreEqual(CodeItemKind.Function, entry.CodeItems[2].Kind);
            Assert.AreEqual("CreateInputField", entry.CodeItems[2].Name);
            Assert.AreEqual(12, entry.CodeItems[2].LineNumber);

            Assert.HasCount(3, entry.CodeChunks);

            Assert.AreEqual("InputField", entry.CodeChunks[0].Name);
            Assert.Contains("class InputField", entry.CodeChunks[0].Text);

            Assert.AreEqual("New", entry.CodeChunks[1].Name);
            Assert.Contains("method New(xValue) class InputField", entry.CodeChunks[1].Text);

            Assert.AreEqual("CreateInputField", entry.CodeChunks[2].Name);
            Assert.Contains("function CreateInputField()", entry.CodeChunks[2].Text);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }

    [TestMethod]
    public void Scan_ShouldIndexRealisticCSharpFileWithCodeItemsAndChunks()
    {
        string rootPath = Path.Combine(
            Path.GetTempPath(),
            $"LegacyLensTests_{Guid.NewGuid()}"
        );

        try
        {
            Directory.CreateDirectory(rootPath);

            string filePath = Path.Combine(rootPath, "FileScanner.cs");

            File.WriteAllLines(
                filePath,
                [
                    "using LegacyLens.Models;",
                    "",
                    "namespace LegacyLens.Services;",
                    "",
                    "public class FileScanner",
                    "{",
                    "    public List<FileIndexEntry> Scan(string rootPath)",
                    "    {",
                    "        return new List<FileIndexEntry>();",
                    "    }",
                    "}",
                    "",
                    "internal sealed record ScanResult",
                    "{",
                    "    public required int FileCount { get; init; }",
                    "}"
                ]
            );

            ScannerOptions options = new ScannerOptions()
            {
                AllowedPatterns = ["*.cs"],
                ExcludedDirectoryNames = ["bin", "obj", ".git", ".vs"],
                SearchOption = SearchOption.AllDirectories
            };

            CodeItemExtractor codeItemExtractor = new CodeItemExtractor();
            CodeChunkBuilder codeChunkBuilder = new CodeChunkBuilder();
            FileScanner scanner = new FileScanner(codeItemExtractor, codeChunkBuilder);

            List<FileIndexEntry> entries = scanner.Scan(rootPath, options);

            Assert.HasCount(1, entries);

            FileIndexEntry entry = entries[0];

            Assert.AreEqual("FileScanner.cs", entry.RelativePath);
            Assert.AreEqual(".cs", entry.Extension);

            Assert.AreEqual("FileScanner.cs#class:FileScanner:5-12", entry.CodeChunks[0].Id);
            Assert.AreEqual("FileScanner.cs#record:ScanResult:13-16", entry.CodeChunks[1].Id);

            Assert.HasCount(2, entry.CodeItems);

            Assert.AreEqual(CodeItemKind.Class, entry.CodeItems[0].Kind);
            Assert.AreEqual("FileScanner", entry.CodeItems[0].Name);
            Assert.AreEqual(5, entry.CodeItems[0].LineNumber);

            Assert.AreEqual(CodeItemKind.Record, entry.CodeItems[1].Kind);
            Assert.AreEqual("ScanResult", entry.CodeItems[1].Name);
            Assert.AreEqual(13, entry.CodeItems[1].LineNumber);

            Assert.HasCount(2, entry.CodeChunks);

            Assert.AreEqual("FileScanner", entry.CodeChunks[0].Name);
            Assert.Contains("public class FileScanner", entry.CodeChunks[0].Text);

            Assert.AreEqual("ScanResult", entry.CodeChunks[1].Name);
            Assert.Contains("internal sealed record ScanResult", entry.CodeChunks[1].Text);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }
}