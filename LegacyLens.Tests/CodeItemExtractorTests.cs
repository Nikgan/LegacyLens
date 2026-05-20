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

    [TestMethod]
    public void Extract_ShouldCalculateCodeItemEndLineNumbers()
    {
        CodeItemExtractor extractor = new CodeItemExtractor();

        string[] lines =
        [
            "function First()",
        "    ? 'first line'",
        "    ? 'second line'",
        "",
        "function Second()",
        "    ? 'another line'",
        "return nil"
        ];

        List<CodeItem> codeItems = extractor.Extract(".prg", lines);

        Assert.HasCount(2, codeItems);

        Assert.AreEqual("First", codeItems[0].Name);
        Assert.AreEqual(1, codeItems[0].LineNumber);
        Assert.AreEqual(4, codeItems[0].EndLineNumber);
        Assert.AreEqual(4, codeItems[0].LineCount);

        Assert.AreEqual("Second", codeItems[1].Name);
        Assert.AreEqual(5, codeItems[1].LineNumber);
        Assert.AreEqual(7, codeItems[1].EndLineNumber);
        Assert.AreEqual(3, codeItems[1].LineCount);
    }
    [TestMethod]
    public void Extract_ShouldIgnoreHarbourMethodDeclarationsInsideClassButFindImplementationsOutside()
    {
        CodeItemExtractor extractor = new CodeItemExtractor();

        string[] lines =
        [
            "class InputField",
        "    method New()",
        "    method Read()",
        "endclass",
        "",
        "method New()",
        "    ::xValue := ''",
        "return Self",
        "",
        "method Read() class InputField",
        "    return nil",
        "",
        "function CreateInputField()",
        "    return InputField():New()"
        ];

        List<CodeItem> codeItems = extractor.Extract(".prg", lines);

        Assert.HasCount(4, codeItems);

        Assert.AreEqual(CodeItemKind.Class, codeItems[0].Kind);
        Assert.AreEqual("InputField", codeItems[0].Name);
        Assert.AreEqual(1, codeItems[0].LineNumber);

        Assert.AreEqual(CodeItemKind.Method, codeItems[1].Kind);
        Assert.AreEqual("New", codeItems[1].Name);
        Assert.AreEqual(6, codeItems[1].LineNumber);

        Assert.AreEqual(CodeItemKind.Method, codeItems[2].Kind);
        Assert.AreEqual("Read", codeItems[2].Name);
        Assert.AreEqual(10, codeItems[2].LineNumber);

        Assert.AreEqual(CodeItemKind.Function, codeItems[3].Kind);
        Assert.AreEqual("CreateInputField", codeItems[3].Name);
        Assert.AreEqual(13, codeItems[3].LineNumber);
    }
}