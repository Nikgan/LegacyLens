using LegacyLens.Models;
using LegacyLens.Services;

namespace LegacyLens.Tests;

[TestClass]
public class CodeChunkBuilderTests
{
    [TestMethod]
    public void Build_ShouldCreateCodeChunkFromCodeItemRange()
    {
        CodeChunkBuilder builder = new CodeChunkBuilder();

        string[] lines =
        [
            "function GetPrice()",
            "    local nPrice := 10",
            "    return nPrice",
            "",
            "function Other()"
        ];

        List<CodeItem> codeItems =
        [
            new CodeItem()
            {
                Kind = CodeItemKind.Function,
                Name = "GetPrice",
                LineNumber = 1,
                EndLineNumber = 3,
                Signature = "function GetPrice()"
            }
        ];

        List<CodeChunk> codeChunks = builder.Build(codeItems, lines);

        Assert.HasCount(1, codeChunks);

        Assert.AreEqual(CodeItemKind.Function, codeChunks[0].Kind);
        Assert.AreEqual("GetPrice", codeChunks[0].Name);
        Assert.AreEqual(1, codeChunks[0].StartLineNumber);
        Assert.AreEqual(3, codeChunks[0].EndLineNumber);
        Assert.AreEqual(3, codeChunks[0].LineCount);

        string expectedText = string.Join(
            Environment.NewLine,
            [
                "function GetPrice()",
                "    local nPrice := 10",
                "    return nPrice"
            ]
        );

        Assert.AreEqual(expectedText, codeChunks[0].Text);
    }

    [TestMethod]
    public void Build_ShouldReturnEmptyListWhenCodeItemsAreEmpty()
    {
        CodeChunkBuilder builder = new CodeChunkBuilder();

        string[] lines =
        [
            "some text",
            "another text"
        ];

        List<CodeItem> codeItems = new List<CodeItem>();

        List<CodeChunk> codeChunks = builder.Build(codeItems, lines);

        Assert.HasCount(0, codeChunks);
    }
}