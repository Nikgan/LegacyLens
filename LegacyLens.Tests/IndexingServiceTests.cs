using LegacyLens.Models;
using LegacyLens.Options;
using LegacyLens.Services;

namespace LegacyLens.Tests;

[TestClass]
public class IndexingServiceTests
{
    [TestMethod]
    public void Build_ShouldUseProvidedSearchOption()
    {
        string rootPath = Path.Combine(
            Path.GetTempPath(),
            $"LegacyLensTests_{Guid.NewGuid()}"
        );

        try
        {
            Directory.CreateDirectory(rootPath);

            File.WriteAllLines(
                Path.Combine(rootPath, "RootFile.cs"),
                [
                    "public class RootFile",
                    "{",
                    "}"
                ]
            );

            string subDirectoryPath = Path.Combine(rootPath, "Sub");
            Directory.CreateDirectory(subDirectoryPath);

            File.WriteAllLines(
                Path.Combine(subDirectoryPath, "SubFile.cs"),
                [
                    "public class SubFile",
                    "{",
                    "}"
                ]
            );

            IndexingService indexingService = CreateIndexingService();

            CodebaseIndex topOnlyIndex = indexingService.Build(
                rootPath,
                SearchOption.TopDirectoryOnly
            );

            CodebaseIndex recursiveIndex = indexingService.Build(
                rootPath,
                SearchOption.AllDirectories
            );

            Assert.AreEqual(1, topOnlyIndex.Summary.FileCount);
            Assert.AreEqual(2, recursiveIndex.Summary.FileCount);
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
    public async Task BuildAsync_ShouldBuildCodebaseIndex()
    {
        string rootPath = Path.Combine(
            Path.GetTempPath(),
            $"LegacyLensTests_{Guid.NewGuid()}"
        );

        Directory.CreateDirectory(rootPath);

        try
        {
            string codeFilePath = Path.Combine(rootPath, "Program.cs");

            File.WriteAllText(
                codeFilePath,
                """
                public class Program
                {
                }
                """
            );

            IndexingService indexingService = CreateIndexingService();

            CodebaseIndex codebaseIndex = await indexingService.BuildAsync(
                rootPath,
                SearchOption.TopDirectoryOnly,
                CancellationToken.None
            );

            Assert.AreEqual(rootPath, codebaseIndex.RootPath);
            Assert.AreEqual(1, codebaseIndex.Summary.FileCount);
            Assert.AreEqual(0, codebaseIndex.Summary.ReadErrorCount);

            Assert.HasCount(1, codebaseIndex.Files);

            FileIndexEntry entry = codebaseIndex.Files[0];

            Assert.AreEqual("Program.cs", entry.RelativePath);
            Assert.AreEqual(".cs", entry.Extension);
            Assert.IsTrue(entry.IsReadSuccessfully);
            Assert.AreEqual(1, entry.CodeItemCount);
            Assert.AreEqual(CodeItemKind.Class, entry.CodeItems[0].Kind);
            Assert.AreEqual("Program", entry.CodeItems[0].Name);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }

    private static IndexingService CreateIndexingService()
    {
        ScannerOptionsFactory scannerOptionsFactory = new();

        CodeItemExtractor codeItemExtractor = new();
        CodeChunkBuilder codeChunkBuilder = new();

        FileScanner fileScanner = new(
            codeItemExtractor,
            codeChunkBuilder
        );

        IndexSummaryBuilder summaryBuilder = new();

        CodebaseIndexBuilder codebaseIndexBuilder = new(
            fileScanner,
            summaryBuilder
        );

        IndexingService indexingService = new(
            scannerOptionsFactory,
            codebaseIndexBuilder
        );

        return indexingService;
    }
}