using LegacyLens.Models;
using LegacyLens.Options;

namespace LegacyLens.Services;

public class FileScanner
{
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
            string[] matchedFiles = Directory.GetFiles(
                directoryPath,
                pattern,
                SearchOption.TopDirectoryOnly
            );

            filePaths.AddRange(matchedFiles);
        }
    }

    private static void AddMatchingFilesFromSubdirectories(
    List<string> filePaths,
    string directoryPath,
    ScannerOptions options)
    {
        string[] subdirectories = Directory.GetDirectories(directoryPath);

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

    private static FileIndexEntry CreateFileIndexEntry(string rootPath, string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);

        string relativePath = Path.GetRelativePath(rootPath, filePath);
        string extension = fileInfo.Extension.ToLowerInvariant();

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            int lineCount = lines.Length;
            int nonEmptyLineCount = CountNonEmptyLines(lines);
            int codeItemCount = CountCodeItems(extension, lines);

            FileIndexEntry entry = new FileIndexEntry()
            {
                FullPath = filePath,
                RelativePath = relativePath,
                Extension = extension,
                SizeBytes = fileInfo.Length,
                LineCount = lineCount,
                NonEmptyLineCount = nonEmptyLineCount,
                CodeItemCount = codeItemCount,
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
                CodeItemCount = 0,
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

    private static int CountCodeItems(string extension, string[] lines)
    {
        int count = 0;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }

            if (trimmedLine.StartsWith("//"))
            {
                continue;
            }

            if (extension == ".prg")
            {
                if (IsHarbourFunctionLikeLine(trimmedLine))
                {
                    count++;
                }
            }

            if (extension == ".cs")
            {
                if (IsCSharpTypeDeclarationLine(trimmedLine))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsHarbourFunctionLikeLine(string line)
    {
        return line.StartsWith("function ", StringComparison.OrdinalIgnoreCase) ||
               line.StartsWith("procedure ", StringComparison.OrdinalIgnoreCase) ||
               line.StartsWith("method ", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCSharpTypeDeclarationLine(string line)
    {
        return line.StartsWith("public class ") ||
               line.StartsWith("internal class ") ||
               line.StartsWith("private class ") ||
               line.StartsWith("protected class ") ||
               line.StartsWith("class ") ||
               line.StartsWith("public record ") ||
               line.StartsWith("internal record ") ||
               line.StartsWith("private record ") ||
               line.StartsWith("record ") ||
               line.StartsWith("public interface ") ||
               line.StartsWith("internal interface ") ||
               line.StartsWith("interface ");
    }
}