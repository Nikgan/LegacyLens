namespace LegacyLens.Options;

public class ScannerOptionsFactory
{
    public ScannerOptions Create(SearchOption searchOption)
    {
        ScannerOptions options = new ScannerOptions()
        {
            AllowedPatterns = ["*.prg", "*.ch", "*.cs", "*.log", "*.txt", "*.md"],
            ExcludedDirectoryNames = ["bin", "obj", ".git", ".vs", ".vscode", "node_modules", "packages"],
            SearchOption = searchOption
        };

        return options;
    }
}