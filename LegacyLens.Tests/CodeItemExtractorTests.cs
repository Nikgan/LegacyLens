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

        Assert.AreEqual("class", codeItems[0].Kind);
        Assert.AreEqual("FileScanner", codeItems[0].Name);
        Assert.AreEqual(1, codeItems[0].LineNumber);

        Assert.AreEqual("record", codeItems[1].Kind);
        Assert.AreEqual("CodebaseIndex", codeItems[1].Name);
        Assert.AreEqual(2, codeItems[1].LineNumber);

        Assert.AreEqual("interface", codeItems[2].Kind);
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

        Assert.AreEqual("function", codeItems[0].Kind);
        Assert.AreEqual("GetPrice", codeItems[0].Name);
        Assert.AreEqual(1, codeItems[0].LineNumber);

        Assert.AreEqual("procedure", codeItems[1].Kind);
        Assert.AreEqual("PrintReport", codeItems[1].Name);
        Assert.AreEqual(2, codeItems[1].LineNumber);

        Assert.AreEqual("method", codeItems[2].Kind);
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
}