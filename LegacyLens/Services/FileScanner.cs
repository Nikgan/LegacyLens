using LegacyLens.Models;
using LegacyLens.Options;

namespace LegacyLens.Services;

public class FileScanner
{
    private readonly CodeItemExtractor _codeItemExtractor;
    private readonly CodeChunkBuilder _codeChunkBuilder;
    public FileScanner(CodeItemExtractor codeItemExtractor, CodeChunkBuilder codeChunkBuilder)
    {
        _codeItemExtractor = codeItemExtractor;
        _codeChunkBuilder = codeChunkBuilder;
    }
    public List<FileIndexEntry> Scan(string rootPath, ScannerOptions options)
    {
        List<FileIndexEntry> entries = new List<FileIndexEntry>();

        List<string> filePaths = GetCandidateFiles(rootPath, options);

        foreach (string filePath in filePaths)
        {
            FileIndexEntry entry = CreateFileIndexEntry(rootPath, filePath);
            entries.Add(entry);
        }

        return entries;
    }

    private static List<string> GetCandidateFiles(string rootPath, ScannerOptions options)
    {
        List<string> filePaths = new List<string>();

        AddMatchingFilesFromDirectory(filePaths, rootPath, options.AllowedPatterns);

        if (options.SearchOption == SearchOption.TopDirectoryOnly)
        {
            return filePaths;
        }

        AddMatchingFilesFromSubdirectories(filePaths, rootPath, options);

        return filePaths;
    }
    private static void AddMatchingFilesFromDirectory(
    List<string> filePaths,
    string directoryPath,
    string[] allowedPatterns)
    {
        foreach (string pattern in allowedPatterns)
        {
            string[] matchedFiles = GetFilesSafe(directoryPath, pattern);
            filePaths.AddRange(matchedFiles);
        }
    }

    private static void AddMatchingFilesFromSubdirectories(
    List<string> filePaths,
    string directoryPath,
    ScannerOptions options)
    {
        string[] subdirectories = GetDirectoriesSafe(directoryPath);

        foreach (string subdirectory in subdirectories)
        {
            if (ShouldSkipDirectory(subdirectory, options.ExcludedDirectoryNames))
            {
                continue;
            }

            AddMatchingFilesFromDirectory(filePaths, subdirectory, options.AllowedPatterns);
            AddMatchingFilesFromSubdirectories(filePaths, subdirectory, options);
        }
    }
    private static string[] GetFilesSafe(string directoryPath, string pattern)
    {
        try
        {
            return Directory.GetFiles(
                directoryPath,
                pattern,
                SearchOption.TopDirectoryOnly
            );
        }
        catch
        {
            return [];
        }
    }

    private static string[] GetDirectoriesSafe(string directoryPath)
    {
        try
        {
            return Directory.GetDirectories(directoryPath);
        }
        catch
        {
            return [];
        }
    }
    private static bool ShouldSkipDirectory(string directoryPath, string[] excludedDirectoryNames)
    {
        string directoryName = Path.GetFileName(directoryPath);

        foreach (string excludedDirectoryName in excludedDirectoryNames)
        {
            if (string.Equals(directoryName, excludedDirectoryName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private FileIndexEntry CreateFileIndexEntry(string rootPath, string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);

        string relativePath = Path.GetRelativePath(rootPath, filePath);
        string extension = fileInfo.Extension.ToLowerInvariant();

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            int lineCount = lines.Length;
            int nonEmptyLineCount = CountNonEmptyLines(lines);
            List<CodeItem> codeItems = _codeItemExtractor.Extract(extension, lines);
            List<CodeChunk> codeChunks = _codeChunkBuilder.Build(codeItems, lines);

            FileIndexEntry entry = new FileIndexEntry()
            {
                FullPath = filePath,
                RelativePath = relativePath,
                Extension = extension,
                SizeBytes = fileInfo.Length,
                LineCount = lineCount,
                NonEmptyLineCount = nonEmptyLineCount,
                CodeItems = codeItems,
                CodeChunks = codeChunks,
                ErrorMessage = null
            };
            return entry;
        }
        catch (Exception exception)
        {
            FileIndexEntry entry = new FileIndexEntry()
            {
                FullPath = filePath,
                RelativePath = relativePath,
                Extension = extension,
                SizeBytes = fileInfo.Exists ? fileInfo.Length : 0,
                LineCount = 0,
                NonEmptyLineCount = 0,
                CodeItems = [],
                CodeChunks = [],
                ErrorMessage = exception.Message
            };
            return entry;
        }
    }

    private static int CountNonEmptyLines(string[] lines)
    {
        int count = 0;

        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                count++;
            }
        }

        return count;
    }
}