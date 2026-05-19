using LegacyLens.Models;
using LegacyLens.Services;

namespace LegacyLens.Tests;

[TestClass]
public class IndexSummaryBuilderTests
{
    [TestMethod]
    public void Build_ShouldCalculateSummary_ForSeveralEntries()
    {
        List<FileIndexEntry> entries = new List<FileIndexEntry>()
        {
            new FileIndexEntry()
            {
                FullPath = @"C:\Project\File1.cs",
                RelativePath = "File1.cs",
                Extension = ".cs",
                SizeBytes = 100,
                LineCount = 10,
                NonEmptyLineCount = 8,
                CodeItemCount = 2,
                ErrorMessage = null
            },
            new FileIndexEntry()
            {
                FullPath = @"C:\Project\File2.prg",
                RelativePath = "File2.prg",
                Extension = ".prg",
                SizeBytes = 200,
                LineCount = 20,
                NonEmptyLineCount = 15,
                CodeItemCount = 3,
                ErrorMessage = null
            }
        };

        IndexSummaryBuilder builder = new IndexSummaryBuilder();

        IndexSummary summary = builder.Build(entries);

        Assert.AreEqual(2, summary.FileCount);
        Assert.AreEqual(0, summary.ReadErrorCount);
        Assert.AreEqual(30, summary.TotalLineCount);
        Assert.AreEqual(23, summary.TotalNonEmptyLineCount);
        Assert.AreEqual(5, summary.totalCodeItemCount);
        Assert.AreEqual(300, summary.TotalSizeBytes);
    }

    [TestMethod]
    public void Build_ShouldCountReadErrors()
    {
        List<FileIndexEntry> entries = new List<FileIndexEntry>()
    {
        new FileIndexEntry()
        {
            FullPath = @"C:\Project\File1.cs",
            RelativePath = "File1.cs",
            Extension = ".cs",
            SizeBytes = 100,
            LineCount = 10,
            NonEmptyLineCount = 8,
            CodeItemCount = 2,
            ErrorMessage = null
        },
        new FileIndexEntry()
        {
            FullPath = @"C:\Project\BrokenFile.cs",
            RelativePath = "BrokenFile.cs",
            Extension = ".cs",
            SizeBytes = 0,
            LineCount = 0,
            NonEmptyLineCount = 0,
            CodeItemCount = 0,
            ErrorMessage = "Access denied"
        }
    };

        IndexSummaryBuilder builder = new IndexSummaryBuilder();

        IndexSummary summary = builder.Build(entries);

        Assert.AreEqual(2, summary.FileCount);
        Assert.AreEqual(1, summary.ReadErrorCount);
        Assert.AreEqual(10, summary.TotalLineCount);
        Assert.AreEqual(8, summary.TotalNonEmptyLineCount);
        Assert.AreEqual(2, summary.totalCodeItemCount);
        Assert.AreEqual(100, summary.TotalSizeBytes);
    }
}