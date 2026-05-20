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
                CodeItems = new List<CodeItem>()
                {
                    new CodeItem()
                    {
                        Kind = CodeItemKind.Class,
                        Name = "TestClass",
                        LineNumber = 1,
                        EndLineNumber = 9,
                        Signature = "public class TestClass"
                    },
                    new CodeItem()
                    {
                        Kind = CodeItemKind.Class,
                        Name = "AnotherClass",
                        LineNumber = 10,
                        EndLineNumber = 12,
                        Signature = "public class AnotherClass"
                    }
                },
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
                CodeItems = new List<CodeItem>()
                {
                    new CodeItem()
                    {
                        Kind = CodeItemKind.Class,
                        Name = "TestClass",
                        LineNumber = 1,
                        EndLineNumber = 9,
                        Signature = "public class TestClass"
                    },
                    new CodeItem()
                    {
                        Kind = CodeItemKind.Class,
                        Name = "AnotherClass",
                        LineNumber = 10,
                        EndLineNumber = 15,
                        Signature = "public class AnotherClass"
                    },
                    new CodeItem()
                    {
                        Kind = CodeItemKind.Class,
                        Name = "Another3Class",
                        LineNumber = 16,
                        EndLineNumber = 19,
                        Signature = "public class AnotherClass"
                    }
                },
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
            CodeItems = new List<CodeItem>()
            {
                new CodeItem()
                {
                    Kind = CodeItemKind.Class,
                    Name = "TestClass",
                    LineNumber = 1,
                    EndLineNumber = 9,
                    Signature = "public class TestClass"
                },
                new CodeItem()
                {
                    Kind = CodeItemKind.Class,
                    Name = "AnotherClass",
                    LineNumber = 10,
                    EndLineNumber = 14,
                    Signature = "public class AnotherClass"
                }
            },
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
            CodeItems = [],
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