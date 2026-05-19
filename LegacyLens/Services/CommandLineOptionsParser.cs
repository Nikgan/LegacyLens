using LegacyLens.Options;

namespace LegacyLens.Services;

public class CommandLineOptionsParser
{
    public CommandLineOptions Parse(string[] args)
    {
        ValidateUnknownOptions(args);

        bool showHelp = HasHelpFlag(args);
        string rootPath = GetRootPath(args);

        CommandLineOptions options = new()
        {
            RootPath = rootPath,
            SearchOption = GetSearchOption(args),
            OutputPath = GetOutputPath(args, rootPath),
            ShowHelp = showHelp
        };

        return options;
    }

    private static void ValidateUnknownOptions(string[] args)
    {
        foreach (string arg in args)
        {
            if (!arg.StartsWith("--") && arg != "-h")
            {
                continue;
            }

            if (arg == "--top" ||
                arg == "--recursive" ||
                arg == "--output" ||
                arg == "--help" ||
                arg == "-h")
            {
                continue;
            }

            throw new ArgumentException($"Unknown option: {arg}");
        }
    }

    private static bool HasHelpFlag(string[] args)
    {
        foreach (string arg in args)
        {
            if (arg == "--help" || arg == "-h")
            {
                return true;
            }
        }

        return false;
    }

    private static string GetRootPath(string[] args)
    {
        if (args.Length > 0 && !args[0].StartsWith("--") && args[0] != "-h")
        {
            return args[0];
        }

        return Directory.GetCurrentDirectory();
    }

    private static SearchOption GetSearchOption(string[] args)
    {
        foreach (string arg in args)
        {
            if (arg == "--top")
            {
                return SearchOption.TopDirectoryOnly;
            }

            if (arg == "--recursive")
            {
                return SearchOption.AllDirectories;
            }
        }

        return SearchOption.AllDirectories;
    }

    private static string GetOutputPath(string[] args, string rootPath)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] != "--output")
            {
                continue;
            }

            int outputPathIndex = i + 1;

            if (outputPathIndex >= args.Length || args[outputPathIndex].StartsWith("--"))
            {
                throw new ArgumentException("Option --output requires a file path.");
            }

            string outputPath = args[outputPathIndex];

            if (!Path.GetExtension(outputPath).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Option --output must point to a .json file.");
            }

            return outputPath;
        }

        return Path.Combine(rootPath, "index.json");
    }
}