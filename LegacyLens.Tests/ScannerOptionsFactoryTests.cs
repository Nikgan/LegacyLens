using LegacyLens.Options;

namespace LegacyLens.Tests;

[TestClass]
public class ScannerOptionsFactoryTests
{
    [TestMethod]
    public void Create_ShouldUseProvidedSearchOption()
    {
        ScannerOptionsFactory factory = new ScannerOptionsFactory();

        ScannerOptions options = factory.Create(SearchOption.TopDirectoryOnly);

        Assert.AreEqual(SearchOption.TopDirectoryOnly, options.SearchOption);
    }

    [TestMethod]
    public void Create_ShouldIncludeDefaultAllowedPatterns()
    {
        ScannerOptionsFactory factory = new ScannerOptionsFactory();

        ScannerOptions options = factory.Create(SearchOption.AllDirectories);

        CollectionAssert.Contains(options.AllowedPatterns, "*.prg");
        CollectionAssert.Contains(options.AllowedPatterns, "*.cs");
        CollectionAssert.Contains(options.AllowedPatterns, "*.md");
    }

    [TestMethod]
    public void Create_ShouldIncludeDefaultExcludedDirectories()
    {
        ScannerOptionsFactory factory = new ScannerOptionsFactory();

        ScannerOptions options = factory.Create(SearchOption.AllDirectories);

        CollectionAssert.Contains(options.ExcludedDirectoryNames, "bin");
        CollectionAssert.Contains(options.ExcludedDirectoryNames, "obj");
        CollectionAssert.Contains(options.ExcludedDirectoryNames, ".git");
        CollectionAssert.Contains(options.ExcludedDirectoryNames, "node_modules");
    }
}