using LegacyLens.Models;
using LegacyLens.Services;

namespace LegacyLens.Tests;

[TestClass]
public class CodeItemExtractorTests
{
    [TestMethod]
    public void Extract_ShouldFindCSharpClassRecordAndInterface()
    {
        CodeItemExtractor extractor = new CodeItemExtractor();

        string[] lines =
        [
            "public class FileScanner",
            "internal record CodebaseIndex",
            "public interface IIndexWriter"
        ];

        List<CodeItem> codeItems = extractor.Extract(".cs", lines);

        Assert.HasCount(3, codeItems);

        Assert.AreEqual(CodeItemKind.Class, codeItems[0].Kind);
        Assert.AreEqual("FileScanner", codeItems[0].Name);
        Assert.AreEqual(1, codeItems[0].LineNumber);

        Assert.AreEqual(CodeItemKind.Record, codeItems[1].Kind);
        Assert.AreEqual("CodebaseIndex", codeItems[1].Name);
        Assert.AreEqual(2, codeItems[1].LineNumber);

        Assert.AreEqual(CodeItemKind.Interface, codeItems[2].Kind);
        Assert.AreEqual("IIndexWriter", codeItems[2].Name);
        Assert.AreEqual(3, codeItems[2].LineNumber);
    }

    [TestMethod]
    public void Extract_ShouldFindHarbourFunctionProcedureAndMethod()
    {
        CodeItemExtractor extractor = new CodeItemExtractor();

        string[] lines =
        [
            "function GetPrice()",
            "procedure PrintReport()",
            "method New() class InputField"
        ];

        List<CodeItem> codeItems = extractor.Extract(".prg", lines);

        Assert.HasCount(3, codeItems);

        Assert.AreEqual(CodeItemKind.Function, codeItems[0].Kind);
        Assert.AreEqual("GetPrice", codeItems[0].Name);
        Assert.AreEqual(1, codeItems[0].LineNumber);

        Assert.AreEqual(CodeItemKind.Procedure, codeItems[1].Kind);
        Assert.AreEqual("PrintReport", codeItems[1].Name);
        Assert.AreEqual(2, codeItems[1].LineNumber);

        Assert.AreEqual(CodeItemKind.Method, codeItems[2].Kind);
        Assert.AreEqual("New", codeItems[2].Name);
        Assert.AreEqual(3, codeItems[2].LineNumber);
    }

    [TestMethod]
    public void Extract_ShouldIgnoreEmptyLinesAndComments()
    {
        CodeItemExtractor extractor = new CodeItemExtractor();

        string[] lines =
        [
            "",
            "   ",
            "// public class FakeClass",
            "public class RealClass"
        ];

        List<CodeItem> codeItems = extractor.Extract(".cs", lines);

        Assert.HasCount(1, codeItems);
        Assert.AreEqual("RealClass", codeItems[0].Name);
        Assert.AreEqual(4, codeItems[0].LineNumber);
    }
    [TestMethod]
    public void Extract_ShouldFindCSharpTypesWithModifiers()
    {
        CodeItemExtractor extractor = new CodeItemExtractor();

        string[] lines =
        [
            "public static class StringUtils",
        "internal sealed record UserDto",
        "public partial class FileScanner",
        "public readonly record struct Point"
        ];

        List<CodeItem> codeItems = extractor.Extract(".cs", lines);

        Assert.HasCount(4, codeItems);

        Assert.AreEqual(CodeItemKind.Class, codeItems[0].Kind);
        Assert.AreEqual("StringUtils", codeItems[0].Name);

        Assert.AreEqual(CodeItemKind.Record, codeItems[1].Kind);
        Assert.AreEqual("UserDto", codeItems[1].Name);

        Assert.AreEqual(CodeItemKind.Class, codeItems[2].Kind);
        Assert.AreEqual("FileScanner", codeItems[2].Name);

        Assert.AreEqual(CodeItemKind.Record, codeItems[3].Kind);
        Assert.AreEqual("Point", codeItems[3].Name);
    }
}