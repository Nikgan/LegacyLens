using LegacyLens.Options;
using LegacyLens.Services;

namespace LegacyLens.Tests;

[TestClass]
public class CommandLineOptionsParserTests
{
    [TestMethod]
    public void Parse_ShouldUseFirstArgumentAsRootPath_WhenFirstArgumentIsNotFlag()
    {
        CommandLineOptionsParser parser = new CommandLineOptionsParser();

        CommandLineOptions options = parser.Parse(["C:\\Project"]);

        Assert.AreEqual("C:\\Project", options.RootPath);
    }

    [TestMethod]
    public void Parse_ShouldUseCurrentDirectory_WhenRootPathIsNotProvided()
    {
        CommandLineOptionsParser parser = new CommandLineOptionsParser();

        CommandLineOptions options = parser.Parse([]);

        Assert.AreEqual(Directory.GetCurrentDirectory(), options.RootPath);
    }

    [TestMethod]
    public void Parse_ShouldUseTopDirectoryOnly_WhenTopFlagIsProvided()
    {
        CommandLineOptionsParser parser = new CommandLineOptionsParser();

        CommandLineOptions options = parser.Parse(["C:\\Project", "--top"]);

        Assert.AreEqual(SearchOption.TopDirectoryOnly, options.SearchOption);
    }

    [TestMethod]
    public void Parse_ShouldUseAllDirectories_WhenRecursiveFlagIsProvided()
    {
        CommandLineOptionsParser parser = new CommandLineOptionsParser();

        CommandLineOptions options = parser.Parse(["C:\\Project", "--recursive"]);

        Assert.AreEqual(SearchOption.AllDirectories, options.SearchOption);
    }

    [TestMethod]
    public void Parse_ShouldUseAllDirectories_ByDefault()
    {
        CommandLineOptionsParser parser = new CommandLineOptionsParser();

        CommandLineOptions options = parser.Parse(["C:\\Project"]);

        Assert.AreEqual(SearchOption.AllDirectories, options.SearchOption);
    }
    [TestMethod]
    public void Parse_ShouldThrowArgumentException_WhenOutputFlagHasNoValue()
    {
        CommandLineOptionsParser parser = new();

        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            parser.Parse(["C:\\Project", "--output"]);
        });
    }
    [TestMethod]
    public void Parse_ShouldUseOutputPath_WhenOutputFlagIsProvided()
    {
        CommandLineOptionsParser parser = new CommandLineOptionsParser();

        CommandLineOptions options = parser.Parse([
            "C:\\Project",
        "--output",
        "C:\\Temp\\project-index.json"
        ]);

        Assert.AreEqual("C:\\Temp\\project-index.json", options.OutputPath);
    }
    [TestMethod]
    public void Parse_ShouldUseDefaultOutputPath_WhenOutputIsNotProvided()
    {
        CommandLineOptionsParser parser = new CommandLineOptionsParser();

        CommandLineOptions options = parser.Parse(["C:\\Project"]);

        Assert.AreEqual(Path.Combine("C:\\Project", "index.json"), options.OutputPath);
    }
}