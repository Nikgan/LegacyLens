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

            ScannerOptionsFactory scannerOptionsFactory = new ScannerOptionsFactory();
            CodeItemExtractor codeItemExtractor = new CodeItemExtractor();
            CodeChunkBuilder codeChunkBuilder = new CodeChunkBuilder();
            FileScanner fileScanner = new FileScanner(codeItemExtractor, codeChunkBuilder);
            IndexSummaryBuilder summaryBuilder = new IndexSummaryBuilder();
            CodebaseIndexBuilder indexBuilder = new CodebaseIndexBuilder(fileScanner, summaryBuilder);
            IndexingService indexingService = new IndexingService(scannerOptionsFactory, indexBuilder);

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
}